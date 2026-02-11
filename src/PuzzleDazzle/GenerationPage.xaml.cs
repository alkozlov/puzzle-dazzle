using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Core.Generation;

namespace PuzzleDazzle;

public partial class GenerationPage : ContentPage
{
	private Maze? _currentMaze;
	private readonly MazeGenerator _generator;

	public GenerationPage()
	{
		InitializeComponent();
		_generator = new MazeGenerator();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Don't auto-generate - user must press Start button
		ShowWelcomeState();
	}

	private void ShowWelcomeState()
	{
		// Show welcome message, hide everything else
		ProgressSection.IsVisible = false;
		MazeDisplayArea.IsVisible = false;
		WelcomeSection.IsVisible = true;
	}

	public async void StartMazeGeneration()
	{
		// Show progress indicator
		WelcomeSection.IsVisible = false;
		ProgressSection.IsVisible = true;
		MazeDisplayArea.IsVisible = false;

		try
		{
			// Get settings from preferences
			int sizeIndex = Preferences.Get("MazeSize", 1); // 0=Small, 1=Medium, 2=Large
			int difficultyIndex = Preferences.Get("MazeDifficulty", 1); // 0=Easy, 1=Medium, 2=Hard

			// Map size index to dimensions
			int rows, columns;
			switch (sizeIndex)
			{
				case 0: // Small
					rows = columns = 10;
					break;
				case 2: // Large
					rows = columns = 30;
					break;
				default: // Medium
					rows = columns = 20;
					break;
			}

			// Map difficulty index to enum
			var difficulty = (MazeDifficulty)difficultyIndex;

			// Generate maze asynchronously
			var progress = new Progress<double>(p =>
			{
				// Progress updates (could show percentage if desired)
			});

			_currentMaze = await _generator.GenerateAsync(rows, columns, difficulty, progress);

			// Display the maze
			MazeView.SetMaze(_currentMaze);

			// Hide progress, show maze
			ProgressSection.IsVisible = false;
			MazeDisplayArea.IsVisible = true;
			
			// Enable Save and Print buttons
			SaveButton.IsEnabled = true;
			PrintButton.IsEnabled = true;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to generate maze: {ex.Message}", "OK");
			ProgressSection.IsVisible = false;
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		// TODO: Implement maze saving functionality
		await DisplayAlert("Save", "Maze will be saved as PNG (coming soon)", "OK");
	}

	private async void OnPrintClicked(object? sender, EventArgs e)
	{
		// TODO: Implement maze printing functionality
		await DisplayAlert("Print", "Maze will be printed (coming soon)", "OK");
	}

	private async void OnSettingsClicked(object? sender, EventArgs e)
	{
		// Navigate to Settings Screen
		await Shell.Current.GoToAsync(nameof(SettingsPage));
	}
}
