
namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService // AdManager Service Methods
{
    public static View GetAdView()
    {
        return AdManager.Instance.GetAdView();
    }

    public bool AreAdsEnabled()
    {
        return AdManager.AreAdsEnabled;
    }

    public async void ShowInterstitialAdAsync()
    {
        await AdManager.Instance.ShowInterstitialAdAsync();
    }

    public async Task ShowInterstitialRewardAdAsync(double timeoutInSeconds = 10
                                                  , bool   showAdAnyway     = false)
    {
        await AdManager.Instance.ShowInterstitialRewardAdAsync(timeoutInSeconds
                                                             , showAdAnyway);
    }

    public async Task ShowRewardAdAsync(double timeoutInSeconds = 10
                                      , bool   showAdAnyway     = false)
    {
        await AdManager.Instance.ShowRewardAdAsync(timeoutInSeconds
                                                 , showAdAnyway);
    }

}
