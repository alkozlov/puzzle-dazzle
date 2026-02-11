namespace PuzzleDazzle.Core.Services;

/// <summary>
/// Service for checking user subscription status.
/// </summary>
public interface ISubscriptionService
{
	/// <summary>
	/// Returns true if the user has an active premium subscription.
	/// </summary>
	Task<bool> IsPremiumAsync();
}
