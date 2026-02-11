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
		// Start maze generation when page appears
		StartMazeGeneration();
	}

	private async void StartMazeGeneration()
	{
		// Show progress indicator
		ProgressSection.IsVisible = true;
		MazeDisplayArea.IsVisible = false;
		SaveButton.IsEnabled = false;
		RetryButton.IsEnabled = false;

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
			SaveButton.IsEnabled = true;
			RetryButton.IsEnabled = true;
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

	private void OnRetryClicked(object? sender, EventArgs e)
	{
		// Regenerate maze with same settings
		StartMazeGeneration();
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		// Navigate back to main page
		await Shell.Current.GoToAsync("..");
	}
}
