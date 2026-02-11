namespace PuzzleDazzle.Services;

/// <summary>
/// Tracks daily maze generation usage for free tier enforcement.
/// Uses MAUI Preferences for persistent storage.
/// </summary>
public class UsageTrackingService
{
	private const string GenerationCountKey = "DailyGenerationCount";
	private const string LastGenerationDateKey = "LastGenerationDate";
	public const int DailyFreeLimit = 5;

	private readonly ISubscriptionService _subscriptionService;

	public UsageTrackingService(ISubscriptionService subscriptionService)
	{
		_subscriptionService = subscriptionService;
	}

	/// <summary>
	/// Returns the number of mazes generated today.
	/// </summary>
	public int GetTodayCount()
	{
		ResetIfNewDay();
		return Preferences.Get(GenerationCountKey, 0);
	}

	/// <summary>
	/// Returns the number of remaining free generations today.
	/// </summary>
	public int GetRemainingCount()
	{
		return Math.Max(0, DailyFreeLimit - GetTodayCount());
	}

	/// <summary>
	/// Returns true if the user can generate a maze.
	/// Premium users always can. Free users are limited to DailyFreeLimit.
	/// </summary>
	public async Task<bool> CanGenerateAsync()
	{
		if (await _subscriptionService.IsPremiumAsync())
			return true;

		return GetRemainingCount() > 0;
	}

	/// <summary>
	/// Records a maze generation. Call this after successful generation.
	/// </summary>
	public void RecordGeneration()
	{
		ResetIfNewDay();
		var count = Preferences.Get(GenerationCountKey, 0);
		Preferences.Set(GenerationCountKey, count + 1);
		Preferences.Set(LastGenerationDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
	}

	/// <summary>
	/// Resets the counter if the stored date is not today.
	/// </summary>
	private void ResetIfNewDay()
	{
		var lastDate = Preferences.Get(LastGenerationDateKey, string.Empty);
		var today = DateTime.Today.ToString("yyyy-MM-dd");

		if (lastDate != today)
		{
			Preferences.Set(GenerationCountKey, 0);
			Preferences.Set(LastGenerationDateKey, today);
		}
	}
}
