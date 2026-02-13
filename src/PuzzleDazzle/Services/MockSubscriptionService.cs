using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Services;

/// <summary>
/// Mock implementation of ISubscriptionService.
/// In DEBUG builds, returns true (premium) for unlimited local development.
/// In RELEASE builds, returns false (free user).
/// Used for testing when Google Play Billing is not available.
/// </summary>
public class MockSubscriptionService : ISubscriptionService
{
	public Task<bool> IsPremiumAsync()
	{
#if DEBUG
		// Debug builds: Unlimited access for local development
		return Task.FromResult(true);
#else
		// Release builds: Free tier with limits
		return Task.FromResult(false);
#endif
	}

	public Task<bool> PurchaseSubscriptionAsync(string productId)
	{
		// Mock implementation always fails (no billing available)
		return Task.FromResult(false);
	}

	public Task RefreshSubscriptionStatusAsync()
	{
		// Mock implementation does nothing
		return Task.CompletedTask;
	}

	public Task<(SubscriptionInfo? Monthly, SubscriptionInfo? Yearly)> GetSubscriptionInfoAsync()
	{
		// Return mock pricing for UI development
		var monthly = new SubscriptionInfo
		{
			ProductId = ISubscriptionService.ProductIdMonthly,
			FormattedPrice = "$0.99",
			BillingPeriod = "month"
		};
		var yearly = new SubscriptionInfo
		{
			ProductId = ISubscriptionService.ProductIdYearly,
			FormattedPrice = "$9.99",
			BillingPeriod = "year"
		};
		return Task.FromResult<(SubscriptionInfo? Monthly, SubscriptionInfo? Yearly)>((monthly, yearly));
	}
}
