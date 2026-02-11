namespace PuzzleDazzle.Services;

/// <summary>
/// Mock implementation of ISubscriptionService.
/// Always returns false (free user). Will be replaced with
/// Google Play Billing implementation in puzzle-e36.
/// </summary>
public class MockSubscriptionService : ISubscriptionService
{
	public Task<bool> IsPremiumAsync()
	{
		return Task.FromResult(false);
	}
}
