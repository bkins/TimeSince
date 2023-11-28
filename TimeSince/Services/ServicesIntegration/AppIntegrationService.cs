namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService
{
    private static AppIntegrationService _instance;
    private const  string                LogTag = "com.hopkins.timeSince.AppIntegrationService";

    // Private service instances
    private readonly AppCenterService _appCenterService;
    private readonly InAppPurchasing  _inAppPurchasing;
    private readonly AdManager        _adManager;

    public static AppIntegrationService Instance => _instance ??= new AppIntegrationService();

    private AppIntegrationService()
    {
        _appCenterService = new AppCenterService();
        _inAppPurchasing  = new InAppPurchasing();
        _adManager        = AdManager.Instance;
    }

    // Method to initialize services
    public void InitializeServices()
    {
        _appCenterService.Start();
        // Initialize other services here if needed
    }
}
