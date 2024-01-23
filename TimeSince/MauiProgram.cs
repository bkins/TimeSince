using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Plugin.MauiMTAdmob;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace TimeSince;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf"
                               , "OpenSansRegular");
                   fonts.AddFont("OpenSans-Semibold.ttf"
                               , "OpenSansSemibold");
               })
               .UseMauiMTAdmob()
               .UseMauiCommunityToolkit()
               .ConfigureSyncfusionCore()
               .UseMauiCompatibility();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
