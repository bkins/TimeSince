namespace TimeSince.Services.ServicesIntegration;


public partial class AppIntegrationService // Device Service Methods
{
    public bool HasInternetAccess { get; set; }

    public async Task<string> GetDoorKay()
    {
        return await _deviceServices.GetDoorKey();
    }

    public bool IsPhysicalDevice()
    {
        return _deviceServices.IsPhysicalDevice();
    }

    public string FullDeviceInfo()
    {
        return _deviceServices.FullDeviceInfo();
    }
}
