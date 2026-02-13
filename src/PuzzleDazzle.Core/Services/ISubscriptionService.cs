namespace PuzzleDazzle.Core.Services;

/// <summary>
/// Represents pricing information for a subscription product
/// retrieved from the billing service (Google Play).
/// </summary>
public class SubscriptionInfo
{
	/// <summary>Product ID (e.g., "premium_monthly").</summary>
	public string ProductId { get; init; } = string.Empty;

	/// <summary>Localized price string (e.g., "$0.99", "0,99 EUR").</summary>
	public string FormattedPrice { get; init; } = string.Empty;

	/// <summary>Billing period description (e.g., "month", "year").</summary>
	public string BillingPeriod { get; init; } = string.Empty;
}

/// <summary>
/// Service for managing user subscriptions.
/// </summary>
public interface ISubscriptionService
{
	/// <summary>Monthly subscription product ID.</summary>
	const string ProductIdMonthly = "premium_monthly";

	/// <summary>Yearly subscription product ID.</summary>
	const string ProductIdYearly = "premium_yearly";

	/// <summary>
	/// Returns true if the user has an active premium subscription.
	/// </summary>
	Task<bool> IsPremiumAsync();

	/// <summary>
	/// Initiates the purchase flow for a subscription.
	/// </summary>
	/// <param name="productId">The subscription product ID (monthly or yearly).</param>
	/// <returns>True if purchase was successful, false otherwise.</returns>
	Task<bool> PurchaseSubscriptionAsync(string productId);

	/// <summary>
	/// Refreshes the subscription status from the billing service.
	/// </summary>
	Task RefreshSubscriptionStatusAsync();

	/// <summary>
	/// Queries subscription pricing info from the billing service.
	/// Returns null entries if the product is not available.
	/// </summary>
	Task<(SubscriptionInfo? Monthly, SubscriptionInfo? Yearly)> GetSubscriptionInfoAsync();
}
