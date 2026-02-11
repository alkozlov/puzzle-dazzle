namespace PuzzleDazzle;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		LoadSettings();
	}

	private void LoadSettings()
	{
		// Load saved preferences from local storage
		SizePicker.SelectedIndex = Preferences.Get("MazeSize", 1); // Default: Medium
		DifficultyPicker.SelectedIndex = Preferences.Get("MazeDifficulty", 1); // Default: Medium
		
		// Visual style is always Classic for first release
		Preferences.Set("VisualStyle", "Classic");
	}

	private void OnSizeChanged(object? sender, EventArgs e)
	{
		if (SizePicker.SelectedIndex >= 0)
		{
			Preferences.Set("MazeSize", SizePicker.SelectedIndex);
		}
	}

	private void OnDifficultyChanged(object? sender, EventArgs e)
	{
		if (DifficultyPicker.SelectedIndex >= 0)
		{
			Preferences.Set("MazeDifficulty", DifficultyPicker.SelectedIndex);
		}
	}

	private async void OnUpgradeClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(PremiumUpgradePage));
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		// Navigate back to main page
		await Shell.Current.GoToAsync("..");
	}
}
