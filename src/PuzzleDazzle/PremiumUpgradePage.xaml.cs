using PuzzleDazzle.Core.Services;
using PuzzleDazzle.Services;

namespace PuzzleDazzle;

public partial class PremiumUpgradePage : ContentPage
{
	private readonly ISubscriptionService _subscriptionService;
	private SubscriptionInfo? _monthlyInfo;
	private SubscriptionInfo? _yearlyInfo;

	public PremiumUpgradePage()
	{
		InitializeComponent();
		
		// Get subscription service from DI
		_subscriptionService = App.Current?.Handler?.MauiContext?.Services
			.GetService<ISubscriptionService>() ?? new MockSubscriptionService();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Reset UI to loading state each time the page appears
		// This allows retrying if the previous attempt failed
		MonthlyPriceLabel.Text = "...";
		MonthlyPeriodLabel.Text = "";
		MonthlyButton.IsEnabled = true;
		MonthlyButton.Opacity = 1.0;
		AnnualPriceLabel.Text = "...";
		AnnualPeriodLabel.Text = "";
		AnnualButton.IsEnabled = true;
		AnnualButton.Opacity = 1.0;

		await LoadSubscriptionInfoAsync();
	}

	private async Task LoadSubscriptionInfoAsync()
	{
		try
		{
			var (monthly, yearly) = await _subscriptionService.GetSubscriptionInfoAsync();
			_monthlyInfo = monthly;
			_yearlyInfo = yearly;

			if (monthly != null)
			{
				MonthlyPriceLabel.Text = monthly.FormattedPrice;
				MonthlyPeriodLabel.Text = $"/{monthly.BillingPeriod}";
			}
			else
			{
				MonthlyPriceLabel.Text = "N/A";
				MonthlyButton.IsEnabled = false;
				MonthlyButton.Opacity = 0.5;
			}

			if (yearly != null)
			{
				AnnualPriceLabel.Text = yearly.FormattedPrice;
				AnnualPeriodLabel.Text = $"/{yearly.BillingPeriod}";
			}
			else
			{
				AnnualPriceLabel.Text = "N/A";
				AnnualButton.IsEnabled = false;
				AnnualButton.Opacity = 0.5;
			}
		}
		catch
		{
			// If pricing query fails, show fallback and keep buttons enabled
			// The actual purchase flow will query prices again
			MonthlyPriceLabel.Text = "---";
			AnnualPriceLabel.Text = "---";
		}
	}

	private async void OnMonthlyClicked(object? sender, EventArgs e)
	{
		var price = _monthlyInfo?.FormattedPrice ?? "Premium";
		var period = _monthlyInfo?.BillingPeriod ?? "month";
		await PurchaseSubscriptionAsync(ISubscriptionService.ProductIdMonthly, "Monthly", $"{price}/{period}");
	}

	private async void OnAnnualClicked(object? sender, EventArgs e)
	{
		var price = _yearlyInfo?.FormattedPrice ?? "Premium";
		var period = _yearlyInfo?.BillingPeriod ?? "year";
		await PurchaseSubscriptionAsync(ISubscriptionService.ProductIdYearly, "Annual", $"{price}/{period}");
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
		catch (Exception)
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
