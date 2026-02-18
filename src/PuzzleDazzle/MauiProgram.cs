using Microsoft.Extensions.Logging;
using PuzzleDazzle.Core.Services;
using PuzzleDazzle.Rendering;
using PuzzleDazzle.Services;

namespace PuzzleDazzle;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
			});

		// Register shared services
		builder.Services.AddSingleton<IPreferencesService, MauiPreferencesService>();
		builder.Services.AddSingleton<UsageTrackingService>();
		builder.Services.AddSingleton<IMazeRenderer, ClassicMazeRenderer>();
		builder.Services.AddSingleton<MazeExportService>();

#if DEBUG
		builder.Logging.AddDebug();
		
		// Use mock subscription service in debug for unlimited local development
		builder.Services.AddSingleton<ISubscriptionService, MockSubscriptionService>();
#else
		// Use real Google Play Billing in release builds
		builder.Services.AddSingleton<ISubscriptionService, GooglePlaySubscriptionService>();
#endif

		// Register pages for Shell DI-based constructor injection
		builder.Services.AddTransient<GenerationPage>();
		builder.Services.AddTransient<PremiumUpgradePage>();

		return builder.Build();
	}
}
