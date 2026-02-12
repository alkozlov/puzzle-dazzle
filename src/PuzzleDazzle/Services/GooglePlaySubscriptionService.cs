using Android.App;
using Android.BillingClient.Api;
using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Services;

/// <summary>
/// Google Play Billing implementation of ISubscriptionService.
/// Handles subscription purchases, verification, and status checking.
/// </summary>
public class GooglePlaySubscriptionService : Java.Lang.Object, ISubscriptionService, IPurchasesUpdatedListener, IBillingClientStateListener
{
	// Subscription Product IDs (must match Google Play Console configuration)
	public const string PRODUCT_ID_MONTHLY = "premium_monthly";
	public const string PRODUCT_ID_YEARLY = "premium_yearly";

	private BillingClient? _billingClient;
	private Activity? _activity;
	private bool _isPremium = false;
	private TaskCompletionSource<bool>? _purchaseCompletionSource;
	private readonly object _lock = new object();

	/// <summary>
	/// Initializes the billing client with the current activity.
	/// Must be called before using the service.
	/// </summary>
	public void Initialize(Activity activity)
	{
		_activity = activity;
		_billingClient = BillingClient.NewBuilder(activity)
			.SetListener(this)
			.EnablePendingPurchases(PendingPurchasesParams.NewBuilder().EnableOneTimeProducts().Build())
			.Build();

		// Start connection
		_billingClient.StartConnection(this);
	}

	#region IBillingClientStateListener Implementation

	public void OnBillingSetupFinished(BillingResult billingResult)
	{
		if (billingResult.ResponseCode == BillingResponseCode.Ok)
		{
			// Billing client is ready
			// Query existing purchases on startup
			_ = RefreshSubscriptionStatusAsync();
		}
	}

	public void OnBillingServiceDisconnected()
	{
		// Try to restart the connection on the next request
		// The billing client will automatically retry on next operation
	}

	#endregion

	#region IPurchasesUpdatedListener Implementation

	public void OnPurchasesUpdated(BillingResult billingResult, IList<Purchase>? purchases)
	{
		if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
		{
			// Process new purchases
			foreach (var purchase in purchases)
			{
				HandlePurchase(purchase);
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
			// User cancelled the purchase
			lock (_lock)
			{
				_purchaseCompletionSource?.TrySetResult(false);
				_purchaseCompletionSource = null;
			}
		}
		else
		{
			// Error occurred
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
			return false;
		}

		// Ensure billing client is connected
		if (!_billingClient.IsReady)
		{
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
				return false;
			}

			var productDetails = productDetailsResult.ProductDetails.First();

			// Get the offer token (required for subscriptions)
			var offerToken = productDetails.SubscriptionOfferDetails?.FirstOrDefault()?.OfferToken;
			if (string.IsNullOrEmpty(offerToken))
			{
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
			lock (_lock)
			{
				_purchaseCompletionSource = new TaskCompletionSource<bool>();
			}

			// Launch billing flow
			var billingResult = _billingClient.LaunchBillingFlow(_activity, flowParams);

			if (billingResult.ResponseCode != BillingResponseCode.Ok)
			{
				lock (_lock)
				{
					_purchaseCompletionSource = null;
				}
				return false;
			}

			// Wait for purchase completion
			TaskCompletionSource<bool>? tcs;
			lock (_lock)
			{
				tcs = _purchaseCompletionSource;
			}

			if (tcs != null)
			{
				return await tcs.Task;
			}

			return false;
		}
		catch
		{
			return false;
		}
	}

	public async Task RefreshSubscriptionStatusAsync()
	{
		if (_billingClient == null || !_billingClient.IsReady)
		{
			_isPremium = false;
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
					(p.Products.Contains(PRODUCT_ID_MONTHLY) || p.Products.Contains(PRODUCT_ID_YEARLY)) &&
					p.PurchaseState == PurchaseState.Purchased);

				_isPremium = hasActiveSubscription;

				// Acknowledge any unacknowledged purchases
				foreach (var purchase in purchasesResult.Purchases)
				{
					if (!purchase.IsAcknowledged)
					{
						HandlePurchase(purchase);
					}
				}
			}
			else
			{
				_isPremium = false;
			}
		}
		catch
		{
			_isPremium = false;
		}
	}

	#endregion

	#region Helper Methods

	private void HandlePurchase(Purchase purchase)
	{
		if (purchase.PurchaseState == PurchaseState.Purchased)
		{
			// Mark user as premium
			_isPremium = true;

			// Acknowledge the purchase if not already acknowledged
			if (!purchase.IsAcknowledged)
			{
				var acknowledgePurchaseParams = AcknowledgePurchaseParams.NewBuilder()
					.SetPurchaseToken(purchase.PurchaseToken)
					.Build();

				_billingClient?.AcknowledgePurchaseAsync(acknowledgePurchaseParams);
			}
		}
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
