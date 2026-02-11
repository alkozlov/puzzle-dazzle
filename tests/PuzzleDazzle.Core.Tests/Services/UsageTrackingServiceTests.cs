using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Core.Tests.Services;

/// <summary>
/// In-memory implementation of IPreferencesService for testing.
/// </summary>
public class InMemoryPreferencesService : IPreferencesService
{
	private readonly Dictionary<string, object> _store = new();

	public int GetInt(string key, int defaultValue)
		=> _store.TryGetValue(key, out var val) ? (int)val : defaultValue;

	public void SetInt(string key, int value)
		=> _store[key] = value;

	public string GetString(string key, string defaultValue)
		=> _store.TryGetValue(key, out var val) ? (string)val : defaultValue;

	public void SetString(string key, string value)
		=> _store[key] = value;
}

/// <summary>
/// Configurable mock subscription service for testing.
/// </summary>
public class TestSubscriptionService : ISubscriptionService
{
	public bool IsPremium { get; set; } = false;

	public Task<bool> IsPremiumAsync() => Task.FromResult(IsPremium);
}

public class UsageTrackingServiceTests
{
	private readonly InMemoryPreferencesService _prefs;
	private readonly TestSubscriptionService _subscription;
	private readonly UsageTrackingService _service;

	public UsageTrackingServiceTests()
	{
		_prefs = new InMemoryPreferencesService();
		_subscription = new TestSubscriptionService();
		_service = new UsageTrackingService(_subscription, _prefs);
	}

	[Fact]
	public void GetTodayCount_ReturnsZero_WhenNoGenerations()
	{
		Assert.Equal(0, _service.GetTodayCount());
	}

	[Fact]
	public void GetRemainingCount_ReturnsDailyLimit_WhenNoGenerations()
	{
		Assert.Equal(UsageTrackingService.DailyFreeLimit, _service.GetRemainingCount());
	}

	[Fact]
	public void RecordGeneration_IncrementsTodayCount()
	{
		_service.RecordGeneration();
		Assert.Equal(1, _service.GetTodayCount());

		_service.RecordGeneration();
		Assert.Equal(2, _service.GetTodayCount());
	}

	[Fact]
	public void RecordGeneration_DecreasesRemainingCount()
	{
		var initial = _service.GetRemainingCount();
		_service.RecordGeneration();
		Assert.Equal(initial - 1, _service.GetRemainingCount());
	}

	[Fact]
	public void GetRemainingCount_NeverGoesNegative()
	{
		for (int i = 0; i < UsageTrackingService.DailyFreeLimit + 3; i++)
		{
			_service.RecordGeneration();
		}

		Assert.Equal(0, _service.GetRemainingCount());
	}

	[Fact]
	public async Task CanGenerateAsync_ReturnsTrue_WhenUnderLimit()
	{
		Assert.True(await _service.CanGenerateAsync());
	}

	[Fact]
	public async Task CanGenerateAsync_ReturnsFalse_WhenLimitReached()
	{
		for (int i = 0; i < UsageTrackingService.DailyFreeLimit; i++)
		{
			_service.RecordGeneration();
		}

		Assert.False(await _service.CanGenerateAsync());
	}

	[Fact]
	public async Task CanGenerateAsync_ReturnsTrue_WhenPremium_EvenAtLimit()
	{
		for (int i = 0; i < UsageTrackingService.DailyFreeLimit; i++)
		{
			_service.RecordGeneration();
		}

		_subscription.IsPremium = true;
		Assert.True(await _service.CanGenerateAsync());
	}

	[Fact]
	public void DailyFreeLimit_IsFive()
	{
		Assert.Equal(5, UsageTrackingService.DailyFreeLimit);
	}

	[Fact]
	public void ResetIfNewDay_ResetsCount_WhenDateChanges()
	{
		// Simulate some generations
		_service.RecordGeneration();
		_service.RecordGeneration();
		Assert.Equal(2, _service.GetTodayCount());

		// Simulate a date change by setting the stored date to yesterday
		_prefs.SetString("LastGenerationDate", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));

		// Count should be reset on next access
		Assert.Equal(0, _service.GetTodayCount());
	}

	[Fact]
	public void ResetIfNewDay_ResetsRemainingCount_WhenDateChanges()
	{
		// Use up all generations
		for (int i = 0; i < UsageTrackingService.DailyFreeLimit; i++)
		{
			_service.RecordGeneration();
		}
		Assert.Equal(0, _service.GetRemainingCount());

		// Simulate next day
		_prefs.SetString("LastGenerationDate", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));

		// Should be reset to full limit
		Assert.Equal(UsageTrackingService.DailyFreeLimit, _service.GetRemainingCount());
	}

	[Fact]
	public async Task CanGenerateAsync_ReturnsTrue_AfterDayReset()
	{
		// Exhaust limit
		for (int i = 0; i < UsageTrackingService.DailyFreeLimit; i++)
		{
			_service.RecordGeneration();
		}
		Assert.False(await _service.CanGenerateAsync());

		// Simulate next day
		_prefs.SetString("LastGenerationDate", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));

		// Should be able to generate again
		Assert.True(await _service.CanGenerateAsync());
	}
}
