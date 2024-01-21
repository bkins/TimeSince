using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Controls;
using TimeSince.Avails;
using TimeSince.Data;
using View = Microsoft.Maui.Controls.View;

namespace TimeSince.Services;

public partial class AdManager
{
    private MTAdView _bannerAdView;

    private static AdManager _instance;
    public static  AdManager Instance => _instance ??= new AdManager();

    public static bool AreAdsEnabled => DetermineIfAdsAreEnabled();

    private static bool DetermineIfAdsAreEnabled()
    {
        // Check if the user has paid to remove ads
        var removalOfAdAsNotBeenPaidFor = ! PreferencesDataStore.PaidToTurnOffAds;

        // Check if the device being used is a physical device
        var isPhysicalDevice = App.AppServiceMethods.IsPhysicalDevice();

        // Determine if ads should be enabled based on the conditions
        var enableAds = removalOfAdAsNotBeenPaidFor
                     || isPhysicalDevice;

        return enableAds;
    }

    private static readonly Secrets.FileJsonContentProvider FileJsonContentProvider = new();

    private readonly Secrets _secrets = new(FileJsonContentProvider.GetJsonContent);

    private string BannerAdUnitId => _secrets.GetSecretValue(SecretCollections.Admob
                                                           , SecretKeys.MainPageBanner);

    private string InterstitialAdUnitId => _secrets.GetSecretValue(SecretCollections.Admob
                                                                 , SecretKeys.MainPageNewEventInterstitial);

    private string RewardedAdUnitId => _secrets.GetSecretValue(SecretCollections.Admob
                                                             , SecretKeys.MainPageRewarded);

    private AdManager()
    {
        InitializeBannerAd();

        InitializeSubscriptions();
    }

    private void InitializeSubscriptions()
    {
        CrossMauiMTAdmob.Current.OnInterstitialClicked    += CurrentOnInterstitialClicked;
        CrossMauiMTAdmob.Current.OnInterstitialClosed     += CurrentOnInterstitialClosed;
        CrossMauiMTAdmob.Current.OnInterstitialImpression += CurrentOnInterstitialImpression;
        CrossMauiMTAdmob.Current.OnInterstitialLoaded     += CurrentOnInterstitialLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialOpened     += CurrentOnInterstitialOpened;

        CrossMauiMTAdmob.Current.OnRewardedClicked      += CurrentOnRewardedClicked;
        CrossMauiMTAdmob.Current.OnRewardedClosed       += CurrentOnRewardedClosed;
        CrossMauiMTAdmob.Current.OnRewardedImpression   += CurrentOnRewardedImpression;
        CrossMauiMTAdmob.Current.OnRewardedLoaded       += CurrentOnRewardedLoaded;
        CrossMauiMTAdmob.Current.OnRewardedOpened       += CurrentOnRewardedOpened;
        CrossMauiMTAdmob.Current.OnRewardedFailedToLoad += CurrentOnRewardedFailedToLoad;
        CrossMauiMTAdmob.Current.OnRewardedFailedToShow += CurrentOnRewardedFailedToShow;
        CrossMauiMTAdmob.Current.OnUserEarnedReward     += CurrentOnUserEarnedReward;
    }

    private void InitializeBannerAd()
    {
        _bannerAdView = new MTAdView
                        {
                            AdsId             = BannerAdUnitId
                          , HorizontalOptions = LayoutOptions.Center
                          , VerticalOptions   = LayoutOptions.End
                        };

        _bannerAdView.AdsLoaded       += OnBannerAdLoaded;
        _bannerAdView.AdsFailedToLoad += OnBannerAdFailedToLoad;
    }

    public View GetAdView()
    {
        return AreAdsEnabled
                ? _bannerAdView
                : null;

    }

    private async Task LoadAdAsync(Action     loadFunction
                                 , Func<bool> isLoadedFunction
                                 , Action     showFunction
                                 , double     timeoutInSeconds)
    {
        loadFunction();

        var timeoutDelay      = TimeSpan.FromSeconds(timeoutInSeconds);
        var cancellationToken = new CancellationTokenSource(timeoutDelay);

        try
        {
            while (! isLoadedFunction())
            {
                await Task.Delay(1000
                               , cancellationToken.Token);
                //await Task.Delay(1000);

                 cancellationToken.Token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException canceledException)
        {
            App.Logger.LogEvent($"{canceledException.Message} (Ad timed out)", null); //(canceledException.Message, string.Empty, string.Empty, null);

            return;
        }
        catch (Exception exception)
        {
            App.Logger.LogError(exception.Message, string.Empty, string.Empty, null);

            return;
        }

        // If the ad loaded successfully, show it
        showFunction();
    }

    public async Task ShowInterstitialAdAsync(double timeoutInSeconds = 15
                                            , bool   showAdAnyway     = false)
    {
#if DEBUG
        WarnDeveloperAboutAdsOnEmulatedDevice(showAdAnyway);
#endif
        // If ads are disabled and don't show ad anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync(() => CrossMauiMTAdmob.Current.LoadInterstitial(InterstitialAdUnitId)
                        , () => CrossMauiMTAdmob.Current.IsInterstitialLoaded()
                        , () => CrossMauiMTAdmob.Current.ShowInterstitial()
                        , timeoutInSeconds);
    }

    public async Task ShowInterstitialRewardAdAsync(double timeoutInSeconds = 10
                                                  , bool   showAdAnyway     = false)
    {

#if DEBUG
        WarnDeveloperAboutAdsOnEmulatedDevice(showAdAnyway);
#endif

        // If ads are disabled and don't show ad anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync(() => CrossMauiMTAdmob.Current.LoadRewardInterstitial(InterstitialAdUnitId)
                        , () => CrossMauiMTAdmob.Current.IsRewardInterstitialLoaded()
                        , () => CrossMauiMTAdmob.Current.ShowRewardInterstitial()
                        , timeoutInSeconds);
    }

    private static void WarnDeveloperAboutAdsOnEmulatedDevice(bool showAdAnyway)
    {
        if (showAdAnyway
         && ! App.AppServiceMethods.IsPhysicalDevice())
        {
            App.Logger.ToastMessage("Ads may not display properly on an emulated device.");
        }
    }

    public async Task ShowRewardAdAsync(double timeoutInSeconds = 10
                                      , bool   showAdAnyway     = false)
    {
#if DEBUG
        WarnDeveloperAboutAdsOnEmulatedDevice(showAdAnyway);
#endif

        // If ads are disabled and don't show ad anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync(() => CrossMauiMTAdmob.Current.LoadRewarded(RewardedAdUnitId)
                        , () => CrossMauiMTAdmob.Current.IsRewardedLoaded()
                        , () => CrossMauiMTAdmob.Current.ShowRewarded()
                        , timeoutInSeconds);
    }
}
