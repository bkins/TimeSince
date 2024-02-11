namespace TimeSince.Services.ServicesIntegration;


public partial class AppIntegrationService // Device Service Methods
{
    public bool HasInternetAccess { get; set; }

    public async Task<string?> GetDoorKay()
    {
        return await DeviceServices.GetDoorKey();
    }

    public static bool IsPhysicalDevice()
    {
        return DeviceServices.IsPhysicalDevice();
    }

    public static string FullDeviceInfo()
    {
        return DeviceServices.FullDeviceInfo();
    }

    public void ComposeEmail(string body, string toEmailAddress, string subjectPrefix)
    {
        DeviceServices.ComposeEmail(body, toEmailAddress, subjectPrefix);
    }
}
