using Android.App;
using Android.BillingClient.Api;
using Microsoft.Extensions.Logging;
using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Services;

/// <summary>
/// Google Play Billing implementation of ISubscriptionService.
/// Handles subscription purchases, verification, and status checking.
/// </summary>
public class GooglePlaySubscriptionService : Java.Lang.Object, ISubscriptionService, IPurchasesUpdatedListener, IBillingClientStateListener
{
	private const string PremiumStatusKey = "IsPremiumUser";
	private const int MaxAcknowledgeRetries = 3;
	private const int MaxReconnectRetries = 5;
	private const int BillingConnectionTimeoutMs = 10_000;

	private BillingClient? _billingClient;
	private Activity? _activity;
	private bool _isPremium;
	private TaskCompletionSource<bool>? _purchaseCompletionSource;
	private TaskCompletionSource<bool> _connectionReady = new();
	private readonly object _lock = new object();
	private readonly ILogger<GooglePlaySubscriptionService> _logger;
	private readonly IPreferencesService _preferences;
	private int _reconnectAttempt = 0;

	public GooglePlaySubscriptionService(
		ILogger<GooglePlaySubscriptionService> logger,
		IPreferencesService preferences)
	{
		_logger = logger;
		_preferences = preferences;

		// Load cached premium status so paying users get immediate access on cold start
		_isPremium = _preferences.GetBool(PremiumStatusKey, false);
	}

	/// <summary>
	/// Initializes the billing client with the current activity.
	/// Must be called before using the service.
	/// </summary>
	public void Initialize(Activity activity)
	{
		_activity = activity;
		_billingClient = BillingClient.NewBuilder(activity)
			.SetListener(this)
			.EnablePendingPurchases(
				PendingPurchasesParams.NewBuilder()
					.EnableOneTimeProducts()
					.EnablePrepaidPlans()
					.Build())
			.Build();

		// Start connection
		_billingClient.StartConnection(this);
	}

	#region IBillingClientStateListener Implementation

	public void OnBillingSetupFinished(BillingResult billingResult)
	{
		if (billingResult.ResponseCode == BillingResponseCode.Ok)
		{
			_logger.LogInformation("Billing client connected successfully");
			_reconnectAttempt = 0;
			_connectionReady.TrySetResult(true);

			// Query existing purchases on startup to sync status with Google Play
			_ = RefreshSubscriptionStatusAsync();
		}
		else
		{
			_logger.LogWarning("Billing setup failed with code {ResponseCode}: {DebugMessage}",
				billingResult.ResponseCode, billingResult.DebugMessage);
			_connectionReady.TrySetResult(false);
		}
	}

	public void OnBillingServiceDisconnected()
	{
		_logger.LogWarning("Billing service disconnected (attempt {Attempt}/{Max})",
			_reconnectAttempt + 1, MaxReconnectRetries);

		// Reset connection readiness so callers will wait for reconnection
		_connectionReady = new TaskCompletionSource<bool>();

		// Retry connection with exponential backoff
		if (_reconnectAttempt < MaxReconnectRetries)
		{
			var delayMs = (int)Math.Pow(2, _reconnectAttempt) * 1000; // 1s, 2s, 4s, 8s, 16s
			_reconnectAttempt++;

			_ = Task.Run(async () =>
			{
				await Task.Delay(delayMs);
				try
				{
					_billingClient?.StartConnection(this);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to reconnect billing client on attempt {Attempt}",
						_reconnectAttempt);
				}
			});
		}
		else
		{
			_logger.LogError("Billing client reconnection failed after {Max} attempts", MaxReconnectRetries);
		}
	}

	#endregion

	#region IPurchasesUpdatedListener Implementation

	public void OnPurchasesUpdated(BillingResult billingResult, IList<Purchase>? purchases)
	{
		if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
		{
			_logger.LogInformation("Purchases updated: {Count} purchase(s) received", purchases.Count);

			// Process new purchases
			foreach (var purchase in purchases)
			{
				_ = HandlePurchaseAsync(purchase);
			}

			// Complete the purchase task
			lock (_lock)
			{
				_purchaseCompletionSource?.TrySetResult(true);
				_purchaseCompletionSource = null;
			}
		}
		else if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
		{
			_logger.LogInformation("User cancelled the purchase flow");
			lock (_lock)
			{
				_purchaseCompletionSource?.TrySetResult(false);
				_purchaseCompletionSource = null;
			}
		}
		else if (billingResult.ResponseCode == BillingResponseCode.ItemAlreadyOwned)
		{
			_logger.LogInformation("Item already owned, refreshing subscription status");
			// User already has this subscription - refresh status
			SetPremiumStatus(true);
			lock (_lock)
			{
				_purchaseCompletionSource?.TrySetResult(true);
				_purchaseCompletionSource = null;
			}
		}
		else
		{
			_logger.LogWarning("Purchase update failed with code {ResponseCode}: {DebugMessage}",
				billingResult.ResponseCode, billingResult.DebugMessage);
			lock (_lock)
			{
				_purchaseCompletionSource?.TrySetResult(false);
				_purchaseCompletionSource = null;
			}
		}
	}

	#endregion

	#region ISubscriptionService Implementation

	public Task<bool> IsPremiumAsync()
	{
		return Task.FromResult(_isPremium);
	}

	public async Task<bool> PurchaseSubscriptionAsync(string productId)
	{
		if (_billingClient == null || _activity == null)
		{
			_logger.LogWarning("Cannot purchase: billing client or activity is null");
			return false;
		}

		// Wait for billing client to be connected (if still connecting)
		if (!await WaitForConnectionAsync())
		{
			_logger.LogWarning("Cannot purchase: billing client failed to connect");
			return false;
		}

		try
		{
			// Query product details
			var productList = new List<QueryProductDetailsParams.Product>
			{
				QueryProductDetailsParams.Product.NewBuilder()
					.SetProductId(productId)
					.SetProductType(BillingClient.ProductType.Subs)
					.Build()
			};

			var productParams = QueryProductDetailsParams.NewBuilder()
				.SetProductList(productList)
				.Build();

			var productDetailsResult = await _billingClient.QueryProductDetailsAsync(productParams);

			if (productDetailsResult.Result.ResponseCode != BillingResponseCode.Ok ||
				productDetailsResult.ProductDetails == null ||
				!productDetailsResult.ProductDetails.Any())
			{
				_logger.LogWarning("Failed to query product details for {ProductId}: {ResponseCode}",
					productId, productDetailsResult.Result.ResponseCode);
				return false;
			}

			var productDetails = productDetailsResult.ProductDetails.First();

			// Get the offer token (required for subscriptions)
			var subscriptionOffers = productDetails.GetSubscriptionOfferDetails();
			if (subscriptionOffers == null || !subscriptionOffers.Any())
			{
				_logger.LogWarning("No subscription offers found for {ProductId}", productId);
				return false;
			}

			var offerToken = subscriptionOffers.First().OfferToken;
			if (string.IsNullOrEmpty(offerToken))
			{
				_logger.LogWarning("Empty offer token for {ProductId}", productId);
				return false;
			}

			// Build purchase flow params
			var productDetailsParamsList = new List<BillingFlowParams.ProductDetailsParams>
			{
				BillingFlowParams.ProductDetailsParams.NewBuilder()
					.SetProductDetails(productDetails)
					.SetOfferToken(offerToken)
					.Build()
			};

			var flowParams = BillingFlowParams.NewBuilder()
				.SetProductDetailsParamsList(productDetailsParamsList)
				.Build();

			// Create task completion source for purchase result
			TaskCompletionSource<bool> tcs;
			lock (_lock)
			{
				tcs = new TaskCompletionSource<bool>();
				_purchaseCompletionSource = tcs;
			}

			// Launch billing flow
			var billingResult = _billingClient.LaunchBillingFlow(_activity, flowParams);

			if (billingResult.ResponseCode != BillingResponseCode.Ok)
			{
				_logger.LogWarning("LaunchBillingFlow failed with code {ResponseCode}: {DebugMessage}",
					billingResult.ResponseCode, billingResult.DebugMessage);
				lock (_lock)
				{
					_purchaseCompletionSource = null;
				}
				return false;
			}

			// Wait for purchase completion via the OnPurchasesUpdated callback
			return await tcs.Task;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during purchase flow for {ProductId}", productId);
			return false;
		}
	}

	public async Task RefreshSubscriptionStatusAsync()
	{
		if (_billingClient == null || !_billingClient.IsReady)
		{
			_logger.LogDebug("Cannot refresh subscription status: billing client not ready, using cached status");
			// Keep the cached _isPremium value instead of resetting to false
			return;
		}

		try
		{
			// Query active subscriptions
			var purchasesResult = await _billingClient.QueryPurchasesAsync(
				QueryPurchasesParams.NewBuilder()
					.SetProductType(BillingClient.ProductType.Subs)
					.Build()
			);

			if (purchasesResult.Result.ResponseCode == BillingResponseCode.Ok &&
				purchasesResult.Purchases != null)
			{
				// Check if user has any active subscription
				var hasActiveSubscription = purchasesResult.Purchases.Any(p =>
					(p.Products.Contains(ISubscriptionService.ProductIdMonthly) ||
					 p.Products.Contains(ISubscriptionService.ProductIdYearly)) &&
					p.PurchaseState == PurchaseState.Purchased);

				SetPremiumStatus(hasActiveSubscription);

				// Acknowledge any unacknowledged purchases
				foreach (var purchase in purchasesResult.Purchases)
				{
					if (purchase.PurchaseState == PurchaseState.Purchased && !purchase.IsAcknowledged)
					{
						await HandlePurchaseAsync(purchase);
					}
				}

				_logger.LogInformation("Subscription status refreshed: isPremium={IsPremium}", _isPremium);
			}
			else
			{
				_logger.LogWarning("Failed to query purchases: {ResponseCode}",
					purchasesResult.Result.ResponseCode);
				// Keep cached status on query failure instead of revoking access
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error refreshing subscription status");
			// Keep cached status on error instead of revoking access
		}
	}

	public async Task<(SubscriptionInfo? Monthly, SubscriptionInfo? Yearly)> GetSubscriptionInfoAsync()
	{
		if (_billingClient == null)
		{
			_logger.LogWarning("Cannot query subscription info: billing client is null");
			return (null, null);
		}

		// Wait for billing client to finish connecting (with timeout)
		if (!await WaitForConnectionAsync())
		{
			_logger.LogWarning("Cannot query subscription info: billing client failed to connect");
			return (null, null);
		}

		try
		{
			var productList = new List<QueryProductDetailsParams.Product>
			{
				QueryProductDetailsParams.Product.NewBuilder()
					.SetProductId(ISubscriptionService.ProductIdMonthly)
					.SetProductType(BillingClient.ProductType.Subs)
					.Build(),
				QueryProductDetailsParams.Product.NewBuilder()
					.SetProductId(ISubscriptionService.ProductIdYearly)
					.SetProductType(BillingClient.ProductType.Subs)
					.Build()
			};

			var productParams = QueryProductDetailsParams.NewBuilder()
				.SetProductList(productList)
				.Build();

			var result = await _billingClient.QueryProductDetailsAsync(productParams);

			if (result.Result.ResponseCode != BillingResponseCode.Ok || result.ProductDetails == null)
			{
				_logger.LogWarning("Failed to query subscription info: {ResponseCode}",
					result.Result.ResponseCode);
				return (null, null);
			}

			SubscriptionInfo? monthly = null;
			SubscriptionInfo? yearly = null;

			foreach (var product in result.ProductDetails)
			{
				var offers = product.GetSubscriptionOfferDetails();
				if (offers == null || !offers.Any()) continue;

				var pricingPhases = offers.First().PricingPhases?.PricingPhaseList;
				if (pricingPhases == null || !pricingPhases.Any()) continue;

				var phase = pricingPhases.First();
				var info = new SubscriptionInfo
				{
					ProductId = product.ProductId,
					FormattedPrice = phase.FormattedPrice ?? string.Empty,
					BillingPeriod = ParseBillingPeriod(phase.BillingPeriod ?? string.Empty)
				};

				if (product.ProductId == ISubscriptionService.ProductIdMonthly)
					monthly = info;
				else if (product.ProductId == ISubscriptionService.ProductIdYearly)
					yearly = info;
			}

			return (monthly, yearly);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying subscription info");
			return (null, null);
		}
	}

	#endregion

	#region Helper Methods

	/// <summary>
	/// Waits for the billing client connection to complete, with a timeout.
	/// Returns true if connected successfully, false if timed out or failed.
	/// </summary>
	private async Task<bool> WaitForConnectionAsync()
	{
		if (_billingClient != null && _billingClient.IsReady)
			return true;

		try
		{
			var timeoutTask = Task.Delay(BillingConnectionTimeoutMs);
			var completedTask = await Task.WhenAny(_connectionReady.Task, timeoutTask);

			if (completedTask == timeoutTask)
			{
				_logger.LogWarning("Billing client connection timed out after {Timeout}ms",
					BillingConnectionTimeoutMs);
				return false;
			}

			return _connectionReady.Task.Result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error waiting for billing client connection");
			return false;
		}
	}

	private async Task HandlePurchaseAsync(Purchase purchase)
	{
		if (purchase.PurchaseState == PurchaseState.Purchased)
		{
			// Mark user as premium
			SetPremiumStatus(true);

			// Acknowledge the purchase if not already acknowledged
			if (!purchase.IsAcknowledged)
			{
				await AcknowledgePurchaseWithRetryAsync(purchase);
			}
		}
		else if (purchase.PurchaseState == PurchaseState.Pending)
		{
			_logger.LogInformation(
				"Purchase is pending (awaiting payment completion) for token {PurchaseToken}",
				purchase.PurchaseToken);
			// Don't grant premium yet — the purchase hasn't been completed.
			// The user will get premium access once the purchase moves to Purchased state
			// on the next OnPurchasesUpdated callback or RefreshSubscriptionStatusAsync call.
		}
	}

	private async Task AcknowledgePurchaseWithRetryAsync(Purchase purchase)
	{
		if (_billingClient == null) return;

		var acknowledgePurchaseParams = AcknowledgePurchaseParams.NewBuilder()
			.SetPurchaseToken(purchase.PurchaseToken)
			.Build();

		for (int attempt = 1; attempt <= MaxAcknowledgeRetries; attempt++)
		{
			try
			{
				var result = await _billingClient.AcknowledgePurchaseAsync(acknowledgePurchaseParams);

				if (result.ResponseCode == BillingResponseCode.Ok)
				{
					_logger.LogInformation("Purchase acknowledged successfully on attempt {Attempt}", attempt);
					return;
				}

				_logger.LogWarning(
					"Acknowledge attempt {Attempt}/{Max} failed with code {ResponseCode}: {DebugMessage}",
					attempt, MaxAcknowledgeRetries, result.ResponseCode, result.DebugMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Acknowledge attempt {Attempt}/{Max} threw an exception",
					attempt, MaxAcknowledgeRetries);
			}

			if (attempt < MaxAcknowledgeRetries)
			{
				var delayMs = (int)Math.Pow(2, attempt) * 1000; // 2s, 4s
				await Task.Delay(delayMs);
			}
		}

		_logger.LogError(
			"Failed to acknowledge purchase after {Max} attempts. " +
			"Purchase token: {PurchaseToken}. This purchase may be refunded after 3 days!",
			MaxAcknowledgeRetries, purchase.PurchaseToken);
	}

	private void SetPremiumStatus(bool isPremium)
	{
		_isPremium = isPremium;
		_preferences.SetBool(PremiumStatusKey, isPremium);
	}

	/// <summary>
	/// Parses an ISO 8601 duration billing period into a human-readable string.
	/// E.g., "P1M" -> "month", "P1Y" -> "year", "P3M" -> "3 months".
	/// </summary>
	private static string ParseBillingPeriod(string isoPeriod)
	{
		if (string.IsNullOrEmpty(isoPeriod)) return string.Empty;

		// ISO 8601 duration format: P[n]Y[n]M[n]D
		var period = isoPeriod.TrimStart('P');

		if (period.EndsWith("M"))
		{
			var months = period.TrimEnd('M');
			if (months == "1") return "month";
			return $"{months} months";
		}

		if (period.EndsWith("Y"))
		{
			var years = period.TrimEnd('Y');
			if (years == "1") return "year";
			return $"{years} years";
		}

		if (period.EndsWith("W"))
		{
			var weeks = period.TrimEnd('W');
			if (weeks == "1") return "week";
			return $"{weeks} weeks";
		}

		if (period.EndsWith("D"))
		{
			var days = period.TrimEnd('D');
			if (days == "1") return "day";
			return $"{days} days";
		}

		return isoPeriod;
	}

	#endregion

	#region Cleanup

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_billingClient?.EndConnection();
			_billingClient?.Dispose();
			_billingClient = null;
		}
		base.Dispose(disposing);
	}

	#endregion
}
