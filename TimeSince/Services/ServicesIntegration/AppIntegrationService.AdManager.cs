
namespace TimeSince.Services.ServicesIntegration;

/// <summary>
/// This class represents an integration service for app-related functionalities such as ads.
/// </summary>
public partial class AppIntegrationService // AdManager Service Methods
{
    public View? GetAdView()
    {
        if (HasInternetAccess) return AdManager.Instance.GetAdView();

        var noInternetLabel = new Label
                              {
                                  Text              = "No internet access"
                                , HorizontalOptions = LayoutOptions.Center
                                , VerticalOptions   = LayoutOptions.Center
                              };
        return noInternetLabel;
    }

    public static bool AreAdsEnabled()
    {
        return AdManager.AreAdsEnabled;
    }

    public async void ShowInterstitialAdAsync()
    {
        if( ! HasInternetAccess) return;

        await AdManager.Instance.ShowInterstitialAdAsync();
    }

    public async Task ShowInterstitialRewardAdAsync(double timeoutInSeconds = 10
                                                  , bool   showAdAnyway     = false)
    {
        if( ! HasInternetAccess) return;

        await AdManager.Instance.ShowInterstitialRewardAdAsync(timeoutInSeconds
                                                             , showAdAnyway);
    }

    public async Task ShowRewardAdAsync(double timeoutInSeconds = 10
                                      , bool   showAdAnyway     = false)
    {
        if( ! HasInternetAccess) return;

        await AdManager.Instance.ShowRewardAdAsync(timeoutInSeconds
                                                 , showAdAnyway);
    }
}
