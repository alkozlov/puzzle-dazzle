using Android.App;
using Android.Content.PM;
using Android.OS;
using PuzzleDazzle.Core.Services;
using PuzzleDazzle.Services;

namespace PuzzleDazzle;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Initialize Google Play Billing service in release builds
#if !DEBUG
		var subscriptionService = MauiApplication.Current.Services.GetService<ISubscriptionService>();
		if (subscriptionService is GooglePlaySubscriptionService billingService)
		{
			billingService.Initialize(this);
		}
#endif
	}
}
