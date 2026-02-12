using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Services;

/// <summary>
/// Mock implementation of ISubscriptionService.
/// In DEBUG builds, returns true (premium) for unlimited local development.
/// In RELEASE builds, returns false (free user).
/// Will be replaced with Google Play Billing implementation in puzzle-e36.
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
}
