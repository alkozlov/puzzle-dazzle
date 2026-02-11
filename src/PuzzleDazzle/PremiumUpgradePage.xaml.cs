namespace PuzzleDazzle;

public partial class PremiumUpgradePage : ContentPage
{
	public PremiumUpgradePage()
	{
		InitializeComponent();
	}

	private async void OnMonthlyClicked(object? sender, EventArgs e)
	{
		// TODO: Integrate with Google Play Billing (puzzle-e36)
		await DisplayAlert(
			"Coming Soon",
			"Monthly subscription ($0.99/month) will be available when the app is published on Google Play.",
			"OK");
	}

	private async void OnAnnualClicked(object? sender, EventArgs e)
	{
		// TODO: Integrate with Google Play Billing (puzzle-e36)
		await DisplayAlert(
			"Coming Soon",
			"Annual subscription ($9.99/year) will be available when the app is published on Google Play.",
			"OK");
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
