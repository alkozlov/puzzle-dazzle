namespace PuzzleDazzle;

public partial class SettingsPage : ContentPage
{
	private readonly Button[] _sizeButtons;
	private readonly Button[] _difficultyButtons;
	private readonly Button[] _shapeButtons;

	public SettingsPage()
	{
		InitializeComponent();

		_sizeButtons = [SizeSmallButton, SizeMediumButton, SizeLargeButton];
		_difficultyButtons = [DifficultyEasyButton, DifficultyMediumButton, DifficultyHardButton];
		_shapeButtons = [ShapeRectangleButton, ShapeCircleButton, ShapeDiamondButton, ShapeHeartButton];

		LoadSettings();
	}

	private void LoadSettings()
	{
		SelectSize(Preferences.Get("MazeSize", 1));            // Default: Medium
		SelectDifficulty(Preferences.Get("MazeDifficulty", 1)); // Default: Medium
		SelectShape(Preferences.Get("MazeShape", 0));           // Default: Rectangle

		// Visual style is always Classic for first release
		Preferences.Set("VisualStyle", "Classic");
	}

	// --- Size ---

	private void OnSizeSmallClicked(object? sender, EventArgs e) => SelectSize(0);
	private void OnSizeMediumClicked(object? sender, EventArgs e) => SelectSize(1);
	private void OnSizeLargeClicked(object? sender, EventArgs e) => SelectSize(2);

	private void SelectSize(int index)
	{
		ApplySelection(_sizeButtons, index);
		Preferences.Set("MazeSize", index);
	}

	// --- Difficulty ---

	private void OnDifficultyEasyClicked(object? sender, EventArgs e) => SelectDifficulty(0);
	private void OnDifficultyMediumClicked(object? sender, EventArgs e) => SelectDifficulty(1);
	private void OnDifficultyHardClicked(object? sender, EventArgs e) => SelectDifficulty(2);

	private void SelectDifficulty(int index)
	{
		ApplySelection(_difficultyButtons, index,
			selectedKey: "DifficultyToggleButtonSelected",
			unselectedKey: "DifficultyToggleButtonUnselected");
		Preferences.Set("MazeDifficulty", index);
	}

	// --- Shape ---

	private void OnShapeRectangleClicked(object? sender, EventArgs e) => SelectShape(0);
	private void OnShapeCircleClicked(object? sender, EventArgs e) => SelectShape(1);
	private void OnShapeDiamondClicked(object? sender, EventArgs e) => SelectShape(2);
	private void OnShapeHeartClicked(object? sender, EventArgs e) => SelectShape(3);

	private void SelectShape(int index)
	{
		ApplySelection(_shapeButtons, index);
		Preferences.Set("MazeShape", index);
	}

	// --- Helpers ---

	/// <summary>
	/// Applies the selected/unselected visual style to a group of toggle buttons.
	/// </summary>
	private void ApplySelection(
		Button[] buttons,
		int selectedIndex,
		string selectedKey = "ToggleButtonSelected",
		string unselectedKey = "ToggleButtonUnselected")
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].Style = (Style)(i == selectedIndex
				? Resources[selectedKey]
				: Resources[unselectedKey]);
		}
	}

	private async void OnUpgradeClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(PremiumUpgradePage));
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
