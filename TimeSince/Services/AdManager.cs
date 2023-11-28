using Microsoft.Extensions.Logging;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Controls;
using TimeSince.Avails;
using TimeSince.Data;

namespace TimeSince.Services;

public partial class AdManager
{
    private MTAdView _bannerAdView;

    private static AdManager _instance;
    public static  AdManager Instance => _instance ??= new AdManager();

    public static bool AreAdsEnabled => PreferencesDataStore.PaidToTurnOffAds;

    private static readonly Secrets.FileJsonContentProvider FileJsonContentProvider = new();

    private Secrets _secrets = new Secrets(FileJsonContentProvider.GetJsonContent);

    private string BannerAdUnitId
    {
        get
        {
            return _secrets.GetSecretValue(SecretCollections.Admob
                                        , SecretKeys.MainPageBanner);
        }
    }

    private string InterstitialAdUnitId
    {
        get
        {
            return _secrets.GetSecretValue(SecretCollections.Admob
                                        , SecretKeys.MainPageNewEventInterstitial);
        }
    }

    private string RewardedAdUnitId
    {
        get
        {
            return _secrets.GetSecretValue(SecretCollections.Admob
                                               , SecretKeys.MainPageRewarded);
        }
    }

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
                await Task.Delay(100
                               , cancellationToken.Token);

                cancellationToken.Token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException canceledException)
        {
           App.Logger.LogError(canceledException.Message);

            return;
        }
        catch (Exception exception)
        {
            App.Logger.LogError(exception.Message);

            return;
        }

        // If the ad loaded successfully, show it
        showFunction();
    }

    public async Task ShowInterstitialAdAsync(double timeoutInSeconds = 10
                                            , bool   showAdAnyway     = false)
    {
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
        // If ads are disabled and don't show ad anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync(() => CrossMauiMTAdmob.Current.LoadRewardInterstitial(InterstitialAdUnitId)
                        , () => CrossMauiMTAdmob.Current.IsRewardInterstitialLoaded()
                        , () => CrossMauiMTAdmob.Current.ShowRewardInterstitial()
                        , timeoutInSeconds);
    }

    public async Task ShowRewardAdAsync(double timeoutInSeconds = 10
                                      , bool   showAdAnyway     = false)
    {
        // If ads are disabled and don't show ad anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync(() => CrossMauiMTAdmob.Current.LoadRewarded(RewardedAdUnitId)
                        , () => CrossMauiMTAdmob.Current.IsRewardedLoaded()
                        , () => CrossMauiMTAdmob.Current.ShowRewarded()
                        , timeoutInSeconds);
    }
}
