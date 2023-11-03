
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Maui.Core.Hosting;
using Google.Apis;
using Plugin.MauiMTAdmob;

namespace TimeSince;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>()
		       .ConfigureFonts(fonts =>
		       {
			       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			       fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		       })
		       .UseMauiMTAdmob()
		       ;

		builder.ConfigureSyncfusionCore();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}


}
