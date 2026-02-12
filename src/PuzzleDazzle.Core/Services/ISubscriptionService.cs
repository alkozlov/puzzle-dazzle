namespace PuzzleDazzle.Core.Services;

/// <summary>
/// Service for managing user subscriptions.
/// </summary>
public interface ISubscriptionService
{
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
}
