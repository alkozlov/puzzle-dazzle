using Microsoft.Extensions.Logging;
using PuzzleDazzle.Core.Services;
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

#if DEBUG
		builder.Logging.AddDebug();
		
		// Use mock subscription service in debug for unlimited local development
		builder.Services.AddSingleton<ISubscriptionService, MockSubscriptionService>();
#else
		// Use real Google Play Billing in release builds
		builder.Services.AddSingleton<ISubscriptionService, GooglePlaySubscriptionService>();
#endif

		return builder.Build();
	}
}
