using System.Text;
using TimeSince.Avails;

namespace TimeSince.Services;

public class DeviceServices
{
    private (NetworkAccess AccessCode, string AccessDescription) InternetAccessStatus()
    {
        var current = Connectivity.NetworkAccess;

        switch (current)
        {
            case NetworkAccess.Internet:
                return (current, "Device has internet access");

            case NetworkAccess.ConstrainedInternet:
            case NetworkAccess.Local:
                return (current, "Device might have limited or local network access");

            case NetworkAccess.None:
                return (current, "No internet access");

            case NetworkAccess.Unknown:
            default:
                return (current, "Unknown - Could not determine the network accessibility");
        }
    }

    public bool HasInternetAccess()
    {
        return InternetAccessStatus().AccessCode == NetworkAccess.Internet;
    }

    public async Task<string?> GetDoorKey()
    {
        return await UiUtilities.GetClipboardValueAsync();
    }

    public bool IsPhysicalDevice()
    {
        return DeviceInfo.Current.DeviceType == DeviceType.Physical;
    }

    public string FullDeviceInfo()
    {
        var infoBuilder = new StringBuilder();
        infoBuilder.AppendLine($"\tModel:         {DeviceInfo.Current.Model}");
        infoBuilder.AppendLine($"\tIdiom:         {DeviceInfo.Current.Idiom}");
        infoBuilder.AppendLine($"\tManufacturer:  {DeviceInfo.Current.Manufacturer}");
        infoBuilder.AppendLine($"\tName:          {DeviceInfo.Current.Name}");
        infoBuilder.AppendLine($"\tVersionString: {DeviceInfo.Current.VersionString}");
        infoBuilder.AppendLine($"\tPlatform:      {DeviceInfo.Current.Platform}");

        return infoBuilder.ToString();
    }

}
