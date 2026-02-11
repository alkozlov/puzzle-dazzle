using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Core.Generation;
using PuzzleDazzle.Services;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle;

public partial class GenerationPage : ContentPage
{
	private Maze? _currentMaze;
	private readonly MazeGenerator _generator;
	private readonly MazeExportService _exportService;

	public GenerationPage()
	{
		InitializeComponent();
		_generator = new MazeGenerator();
		_exportService = new MazeExportService(new ClassicMazeRenderer(), null!);
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

	private async void OnStartClicked(object? sender, EventArgs e)
	{
		await StartMazeGeneration();
	}

	private async Task StartMazeGeneration()
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
			
			// Enable Save and Share buttons
			SaveButton.IsEnabled = true;
			ShareButton.IsEnabled = true;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to generate maze: {ex.Message}", "OK");
			ProgressSection.IsVisible = false;
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		if (_currentMaze == null)
		{
			await DisplayAlert("Error", "No maze to save", "OK");
			return;
		}

		try
		{
			// Capture the maze view as image
			var imageBytes = await _exportService.CaptureViewAsync(MazeView);
			
			// Save to file
			var filePath = await _exportService.SaveToFileAsync(imageBytes);
			
			// Show success message
			await DisplayAlert("Success", $"Maze saved to:\n{filePath}", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to save maze: {ex.Message}", "OK");
		}
	}

	private async void OnShareClicked(object? sender, EventArgs e)
	{
		if (_currentMaze == null)
		{
			await DisplayAlert("Error", "No maze to share", "OK");
			return;
		}

		try
		{
			// Capture the maze view as image
			var imageBytes = await _exportService.CaptureViewAsync(MazeView);
			
			// Save to temp file
			var tempFilePath = await _exportService.SaveToTempFileAsync(imageBytes);
			
			// Share the file
			await Share.Default.RequestAsync(new ShareFileRequest
			{
				Title = "Share Maze",
				File = new ShareFile(tempFilePath)
			});
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to share maze: {ex.Message}", "OK");
		}
	}

	private async void OnSettingsClicked(object? sender, EventArgs e)
	{
		// Navigate to Settings Screen
		await Shell.Current.GoToAsync(nameof(SettingsPage));
	}
}
