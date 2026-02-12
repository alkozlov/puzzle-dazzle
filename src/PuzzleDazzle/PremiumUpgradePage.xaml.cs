using PuzzleDazzle.Core.Services;
using PuzzleDazzle.Services;

namespace PuzzleDazzle;

public partial class PremiumUpgradePage : ContentPage
{
	private readonly ISubscriptionService _subscriptionService;

	public PremiumUpgradePage()
	{
		InitializeComponent();
		
		// Get subscription service from DI
		_subscriptionService = App.Current?.Handler?.MauiContext?.Services
			.GetService<ISubscriptionService>() ?? new MockSubscriptionService();
	}

	private async void OnMonthlyClicked(object? sender, EventArgs e)
	{
		await PurchaseSubscriptionAsync(GooglePlaySubscriptionService.PRODUCT_ID_MONTHLY, "Monthly", "$0.99/month");
	}

	private async void OnAnnualClicked(object? sender, EventArgs e)
	{
		await PurchaseSubscriptionAsync(GooglePlaySubscriptionService.PRODUCT_ID_YEARLY, "Annual", "$9.99/year");
	}

	private async Task PurchaseSubscriptionAsync(string productId, string planName, string price)
	{
		try
		{
			// Initiate purchase flow
			var success = await _subscriptionService.PurchaseSubscriptionAsync(productId);

			if (success)
			{
				await DisplayAlert(
					"Success!",
					$"You now have unlimited maze generation! Thank you for upgrading to {planName} Premium ({price}).",
					"OK");

				// Navigate back to main page
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				// Purchase was cancelled or failed
				// Don't show error if user cancelled
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert(
				"Error",
				"An error occurred while processing your purchase. Please try again.",
				"OK");
		}
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
