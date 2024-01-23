namespace TimeSince.MVVM.Models;

public class AppInfo(string? currentVersion
                   , string? currentBuild)
{
    public string? CurrentVersion { get; } = currentVersion;
    public string? CurrentBuild   { get; } = currentBuild;
}
