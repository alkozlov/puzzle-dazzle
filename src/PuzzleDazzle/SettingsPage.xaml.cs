namespace PuzzleDazzle;

public partial class SettingsPage : ContentPage
{
	private string _selectedStyle = "Classic";

	public SettingsPage()
	{
		InitializeComponent();
		LoadSettings();
		UpdateStyleSelection();
	}

	private void LoadSettings()
	{
		// Load saved preferences from local storage
		// For now, set defaults
		SizePicker.SelectedIndex = Preferences.Get("MazeSize", 1); // Default: Medium
		DifficultyPicker.SelectedIndex = Preferences.Get("MazeDifficulty", 1); // Default: Medium
		_selectedStyle = Preferences.Get("VisualStyle", "Classic");
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

	private void OnClassicStyleTapped(object? sender, EventArgs e)
	{
		_selectedStyle = "Classic";
		Preferences.Set("VisualStyle", _selectedStyle);
		UpdateStyleSelection();
	}

	private void OnSoftStyleTapped(object? sender, EventArgs e)
	{
		_selectedStyle = "Soft";
		Preferences.Set("VisualStyle", _selectedStyle);
		UpdateStyleSelection();
	}

	private void UpdateStyleSelection()
	{
		// Update border color to show selected style
		if (_selectedStyle == "Classic")
		{
			ClassicFrame.BorderColor = Color.FromArgb("#512BD4"); // Primary color
			SoftFrame.BorderColor = Color.FromArgb("#DDD");
		}
		else
		{
			ClassicFrame.BorderColor = Color.FromArgb("#DDD");
			SoftFrame.BorderColor = Color.FromArgb("#512BD4"); // Primary color
		}
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		// Navigate back to main page
		await Shell.Current.GoToAsync("..");
	}
}
