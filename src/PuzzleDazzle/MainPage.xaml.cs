namespace PuzzleDazzle;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnStartClicked(object? sender, EventArgs e)
	{
		// Navigate to Generation Screen
		await Shell.Current.GoToAsync(nameof(GenerationPage));
	}

	private async void OnSettingsClicked(object? sender, EventArgs e)
	{
		// Navigate to Settings Screen
		await Shell.Current.GoToAsync(nameof(SettingsPage));
	}
}
