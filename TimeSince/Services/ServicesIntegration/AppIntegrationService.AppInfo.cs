using AppInfo = TimeSince.MVVM.Models.AppInfo;

namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService
{
    public AppInfo AppInfo => AppInfoService.Info;

    public string GetMode()
    {
        return _appInfoService.GetMode();
    }
}
