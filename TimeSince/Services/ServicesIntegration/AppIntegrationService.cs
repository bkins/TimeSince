﻿namespace TimeSince.Services.ServicesIntegration;

/// <summary>
/// The AppIntegrationService class is responsible for integrating various app services and providing access to them.
/// </summary>
public partial class AppIntegrationService
{
    private const string LogTag = "com.hopkins.timeSince.AppIntegrationService";

    // Private service instances
    private readonly AppCenterService _appCenterService;
    private readonly InAppPurchasing  _inAppPurchasing;
    private readonly AdManager?       _adManager;
    private readonly DeviceServices   _deviceServices;
    private readonly AppInfoService   _appInfoService;
    private readonly SecretsService   _secretsService;

    private static AppIntegrationService? _instance;
    public static  AppIntegrationService  Instance => _instance ??= new AppIntegrationService();

    public static bool IsTesting { get; set; }

    private AppIntegrationService()
    {
        _secretsService   = SecretsService.Instance;
        _appCenterService = new AppCenterService();
        _inAppPurchasing  = new InAppPurchasing();
        _adManager        = AdManager.Instance;
        _deviceServices   = new DeviceServices();
        _appInfoService   = new AppInfoService();
    }

    public void InitializeServices()
    {
        HasInternetAccess = _deviceServices.HasInternetAccess();

        _adManager!.Start();
        _appCenterService.Start();
    }
}
