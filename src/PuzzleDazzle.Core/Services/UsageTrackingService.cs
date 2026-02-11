namespace PuzzleDazzle.Core.Services;

/// <summary>
/// Tracks daily maze generation usage for free tier enforcement.
/// Uses IPreferencesService for persistent storage (testable without MAUI).
/// </summary>
public class UsageTrackingService
{
	private const string GenerationCountKey = "DailyGenerationCount";
	private const string LastGenerationDateKey = "LastGenerationDate";
	public const int DailyFreeLimit = 5;

	private readonly ISubscriptionService _subscriptionService;
	private readonly IPreferencesService _preferences;

	public UsageTrackingService(ISubscriptionService subscriptionService, IPreferencesService preferences)
	{
		_subscriptionService = subscriptionService;
		_preferences = preferences;
	}

	/// <summary>
	/// Returns the number of mazes generated today.
	/// </summary>
	public int GetTodayCount()
	{
		ResetIfNewDay();
		return _preferences.GetInt(GenerationCountKey, 0);
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
		var count = _preferences.GetInt(GenerationCountKey, 0);
		_preferences.SetInt(GenerationCountKey, count + 1);
		_preferences.SetString(LastGenerationDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
	}

	/// <summary>
	/// Resets the counter if the stored date is not today.
	/// </summary>
	private void ResetIfNewDay()
	{
		var lastDate = _preferences.GetString(LastGenerationDateKey, string.Empty);
		var today = DateTime.Today.ToString("yyyy-MM-dd");

		if (lastDate != today)
		{
			_preferences.SetInt(GenerationCountKey, 0);
			_preferences.SetString(LastGenerationDateKey, today);
		}
	}
}
