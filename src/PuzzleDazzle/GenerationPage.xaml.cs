using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Core.Generation;
using PuzzleDazzle.Core.Services;
using PuzzleDazzle.Services;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle;

public partial class GenerationPage : ContentPage
{
	private Maze? _currentMaze;
	private readonly MazeGenerator _generator;
	private readonly MazeExportService _exportService;
	private readonly UsageTrackingService _usageTracking;
	private readonly ISubscriptionService _subscriptionService;

	public GenerationPage()
	{
		InitializeComponent();
		_generator = new MazeGenerator();
		_exportService = new MazeExportService(new ClassicMazeRenderer(), null!);
		_subscriptionService = new MockSubscriptionService();
		_usageTracking = new UsageTrackingService(_subscriptionService, new MauiPreferencesService());
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Don't auto-generate - user must press Start button
		ShowWelcomeState();
		UpdateRemainingLabel();
	}

	private void ShowWelcomeState()
	{
		// Show welcome message, hide everything else
		ProgressSection.IsVisible = false;
		MazeDisplayArea.IsVisible = false;
		WelcomeSection.IsVisible = true;
	}

	private async void UpdateRemainingLabel()
	{
		var remaining = _usageTracking.GetRemainingCount();
		var total = UsageTrackingService.DailyFreeLimit;

		// Hide label for premium users
		if (await _subscriptionService.IsPremiumAsync())
		{
			RemainingLabel.IsVisible = false;
			return;
		}

		RemainingLabel.IsVisible = true;
		RemainingLabel.Text = $"{remaining} of {total} remaining today";

		// Change color when running low
		if (remaining == 0)
			RemainingLabel.TextColor = Color.FromArgb("#FF6B6B");
		else if (remaining <= 2)
			RemainingLabel.TextColor = Color.FromArgb("#FFD700");
		else
			RemainingLabel.TextColor = Color.FromArgb("#999999");
	}

	private async void OnStartClicked(object? sender, EventArgs e)
	{
		// Check if user can generate
		if (!await _usageTracking.CanGenerateAsync())
		{
			var upgrade = await DisplayAlert(
				"Daily Limit Reached",
				$"You've used all {UsageTrackingService.DailyFreeLimit} free mazes today.\n\nUpgrade to Premium for unlimited maze generation!",
				"Upgrade",
				"OK");

			if (upgrade)
			{
				await Shell.Current.GoToAsync(nameof(PremiumUpgradePage));
			}
			return;
		}

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
			int shapeIndex = Preferences.Get("MazeShape", 0); // 0=Rectangle, 1=Circle, 2=Diamond, 3=Heart

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

			// Map shape index to enum and create shape
			var shapeType = (ShapeType)shapeIndex;
			var shape = MazeShape.FromType(shapeType, rows, columns);

			// Generate maze asynchronously using Recursive Backtracking algorithm
			var progress = new Progress<double>(p =>
			{
				// Progress updates (could show percentage if desired)
			});

			_currentMaze = await _generator.GenerateAsync(rows, columns, difficulty, progress, shape);

			// Record the generation for usage tracking
			_usageTracking.RecordGeneration();
			UpdateRemainingLabel();

			// Display the maze
			MazeView.SetMaze(_currentMaze);

			// Hide progress, show maze
			ProgressSection.IsVisible = false;
			MazeDisplayArea.IsVisible = true;
			
			// Enable Save and Share buttons
			SaveButton.IsEnabled = true;
			SaveButton.Opacity = 1.0;
			ShareButton.IsEnabled = true;
			ShareButton.Opacity = 1.0;
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
			// Request storage permissions
			var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
			if (status != PermissionStatus.Granted)
			{
				await DisplayAlert("Permission Required", "Storage permission is needed to save images", "OK");
				return;
			}

			// Capture the maze view as image
			var imageBytes = await _exportService.CaptureViewAsync(MazeView);
			
			// Save to file
			var filePath = await _exportService.SaveToFileAsync(imageBytes);
			
			// Show success message (user-friendly for release, detailed for debug)
#if DEBUG
			await DisplayAlert("Saved!", $"Maze saved to gallery!\n\nDebug path: {filePath}", "OK");
#else
			await DisplayAlert("Saved!", "Your maze has been saved to the gallery!", "OK");
#endif
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

	private async void OnPlayClicked(object? sender, EventArgs e)
	{
		if (_currentMaze == null) return;
		await Navigation.PushAsync(new MazePlayPage(_currentMaze));
	}

	private async void OnSettingsClicked(object? sender, EventArgs e)
	{
		// Navigate to Settings Screen
		await Shell.Current.GoToAsync(nameof(SettingsPage));
	}
}
