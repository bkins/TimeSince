using System.Text;
using TimeSince.Avails;
using static Microsoft.Maui.Devices.DeviceInfo;
using static TimeSince.Services.ServicesIntegration.AppIntegrationService;
using DeviceType = Microsoft.Maui.Devices.DeviceType;

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

    public static async Task<string?> GetDoorKey()
    {
        return await UiUtilities.GetClipboardValueAsync();
    }

    public static bool IsPhysicalDevice()
    {
        return Current.DeviceType == DeviceType.Physical;
    }

    public static string FullDeviceInfo()
    {
        var model        = IsTesting ? "Testing - Model"         : Current.Model;
        var idiom        = IsTesting ? "Testing - Idiom"         : Current.Idiom.ToString();
        var manufacturer = IsTesting ? "Testing - Manufacturer"  : Current.Manufacturer;
        var name         = IsTesting ? "Testing - Name"          : Current.Name;
        var version      = IsTesting ? "Testing - VersionString" : Current.VersionString;
        var platform     = IsTesting ? "Testing - Platform"      : Current.Platform.ToString();

        var infoBuilder  = new StringBuilder();
        infoBuilder.AppendLine($"\tModel:         {model}");
        infoBuilder.AppendLine($"\tIdiom:         {idiom}");
        infoBuilder.AppendLine($"\tManufacturer:  {manufacturer}");
        infoBuilder.AppendLine($"\tName:          {name}");
        infoBuilder.AppendLine($"\tVersionString: {version}");
        infoBuilder.AppendLine($"\tPlatform:      {platform}");

        return infoBuilder.ToString();
    }

    public static async void ComposeEmail(string body, string toEmailAddress, string subjectPrefix)
    {
        try
        {
            var message = new EmailMessage(subjectPrefix
                                         , body
                                         , toEmailAddress);

            await Email.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException notSupportedException)
        {
            App.Logger.LogError(notSupportedException);
            Logger.ToastMessage($"{notSupportedException.Message} (Email not supported on this device)" );
        }
        catch (Exception ex)
        {
            App.Logger.LogError(ex);
        }
    }
}
