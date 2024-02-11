using TimeSince.Services.ServicesIntegration;
using AppInfo = TimeSince.MVVM.Models.AppInfo;

namespace TimeSince.Services;

public class AppInfoService
{
    public static AppInfo? Info { get; set; }

    public AppInfoService()
    {
        var currentVersion = AppIntegrationService.IsTesting ? "0" : VersionTracking.CurrentVersion;
        var currentBuild   = GetBuildName(AppIntegrationService.IsTesting ? "0" : VersionTracking.CurrentBuild);

        Info = new AppInfo(currentVersion, currentBuild);
    }

    public static string GetMode()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }

    private static string? GetBuildName(string buildNumber)
    {
        return buildNumber switch
               {
                   "1" => "Alpha",
                   "2" => "Beta",
                   "3" => "RC",
                   "4" => "Prod",
                   _ => "Unknown"
               };
    }
}
