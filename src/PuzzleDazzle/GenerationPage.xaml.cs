namespace PuzzleDazzle;

public partial class GenerationPage : ContentPage
{
	public GenerationPage()
	{
		InitializeComponent();
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

		// Simulate maze generation (placeholder)
		// TODO: Replace with actual maze generation logic
		await Task.Delay(2000); // Simulate generation time

		// Hide progress, show maze
		ProgressSection.IsVisible = false;
		MazeDisplayArea.IsVisible = true;
		SaveButton.IsEnabled = true;
		RetryButton.IsEnabled = true;
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		// TODO: Implement maze saving functionality
		await DisplayAlert("Save", "Maze will be saved as PNG", "OK");
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
