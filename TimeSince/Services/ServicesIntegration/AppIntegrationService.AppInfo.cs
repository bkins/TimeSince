using AppInfo = TimeSince.MVVM.Models.AppInfo;

namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService
{
    public static AppInfo? AppInfo => AppInfoService.Info;

    public static string GetMode()
    {
        return AppInfoService.GetMode();
    }
}
