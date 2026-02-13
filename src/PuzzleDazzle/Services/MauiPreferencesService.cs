using PuzzleDazzle.Core.Services;

namespace PuzzleDazzle.Services;

/// <summary>
/// MAUI Preferences adapter that implements IPreferencesService.
/// Bridges between the Core abstraction and MAUI's Preferences API.
/// </summary>
public class MauiPreferencesService : IPreferencesService
{
	public int GetInt(string key, int defaultValue) => Preferences.Get(key, defaultValue);
	public void SetInt(string key, int value) => Preferences.Set(key, value);
	public string GetString(string key, string defaultValue) => Preferences.Get(key, defaultValue);
	public void SetString(string key, string value) => Preferences.Set(key, value);
	public bool GetBool(string key, bool defaultValue) => Preferences.Get(key, defaultValue);
	public void SetBool(string key, bool value) => Preferences.Set(key, value);
}
