using AppInfo = TimeSince.MVVM.Models.AppInfo;

namespace TimeSince.Services;

public class AppInfoService
{
    public static AppInfo Info { get; set; }

    public AppInfoService()
    {
        Info = new AppInfo();

        Info.CurrentVersion = VersionTracking.CurrentVersion;
        Info.CurrentBuild   = GetBuildName(VersionTracking.CurrentBuild);
    }

    public string GetMode()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }

    private string GetBuildName(string buildNumber)
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
