using Plugin.MauiMTAdmob.Controls;
using Plugin.MauiMTAdmob;
using TimeSince.Data;

namespace TimeSince.Avails;

public partial class AdManager
{
    private static AdManager _instance;
    public static  AdManager Instance => _instance ??= new AdManager();

    public bool AreAdsEnabled => PreferencesDataStore.PaidToTurnOffAds;

    // AdMob ad unit IDs.
    private static string BannerAdUnitId => Utilities.GetSecretValue(SecretCollections.Admob
                                                                         , SecretKeys.MainPageBanner);
    private static string InterstitialAdUnitId => Utilities.GetSecretValue(SecretCollections.Admob
                                                                         , SecretKeys.MainPageNewEventInterstitial);
    private static string RewardedAdUnitId => Utilities.GetSecretValue(SecretCollections.Admob
                                                                         , SecretKeys.MainPageRewarded);

    private MTAdView _bannerAdView;

    private AdManager()
    {
        InitializeBannerAd();

        InitializeSubscriptions();
    }

    private void InitializeSubscriptions()
    {

        CrossMauiMTAdmob.Current.OnInterstitialClicked    += CurrentOnOnInterstitialClicked;
        CrossMauiMTAdmob.Current.OnInterstitialClosed     += CurrentOnOnInterstitialClosed;
        CrossMauiMTAdmob.Current.OnInterstitialImpression += CurrentOnOnInterstitialImpression;
        CrossMauiMTAdmob.Current.OnInterstitialLoaded     += CurrentOnOnInterstitialLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialOpened     += CurrentOnOnInterstitialOpened;

        CrossMauiMTAdmob.Current.OnRewardedClicked      += CurrentOnOnRewardedClicked;
        CrossMauiMTAdmob.Current.OnRewardedClosed       += CurrentOnOnRewardedClosed;
        CrossMauiMTAdmob.Current.OnRewardedImpression   += CurrentOnOnRewardedImpression;
        CrossMauiMTAdmob.Current.OnRewardedLoaded       += CurrentOnOnRewardedLoaded;
        CrossMauiMTAdmob.Current.OnRewardedOpened       += CurrentOnOnRewardedOpened;
        CrossMauiMTAdmob.Current.OnRewardedFailedToLoad += CurrentOnOnRewardedFailedToLoad;
        CrossMauiMTAdmob.Current.OnRewardedFailedToShow += CurrentOnOnRewardedFailedToShow;
        CrossMauiMTAdmob.Current.OnUserEarnedReward     += CurrentOnOnUserEarnedReward;

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
        if (AreAdsEnabled)
        {
            return _bannerAdView;
        }

        return null;
    }

    public async Task LoadAdAsync(Action loadFunction
                                , Func<bool> isLoadedFunction
                                , Action showFunction
                                , double timeoutInSeconds)
    {
        loadFunction();

        var timeoutDelay      = TimeSpan.FromSeconds(10);
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
        catch (OperationCanceledException)
        {
            //TODO: log this error.  For now I don't care
            return;
        }
        catch (Exception)
        {
            //TODO: log this error.  For now I don't care
            return;
        }

        // If the ad loaded successfully, show it
        showFunction();
    }

    public async Task ShowInterstitialAdAsync(double timeoutInSeconds = 10
                                            , bool   showAdAnyway     = false)
    {
        // If ads are disabled and don't show anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync
        (
            () => CrossMauiMTAdmob.Current.LoadInterstitial(InterstitialAdUnitId)
          , () => CrossMauiMTAdmob.Current.IsInterstitialLoaded()
          , () => CrossMauiMTAdmob.Current.ShowInterstitial()
          , timeoutInSeconds
        );
    }

    public async Task ShowInterstitialRewardAdAsync(double timeoutInSeconds = 10
                                                  , bool   showAdAnyway     = false)
    {
        // If ads are disabled and don't show anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync
        (
             () => CrossMauiMTAdmob.Current.LoadRewardInterstitial(InterstitialAdUnitId)
           , () => CrossMauiMTAdmob.Current.IsRewardInterstitialLoaded()
           , () => CrossMauiMTAdmob.Current.ShowRewardInterstitial()
           , timeoutInSeconds
        );
    }

    public async Task ShowRewardAdAsync(double timeoutInSeconds = 10
                                      , bool   showAdAnyway     = false)
    {
        // If ads are disabled and don't show anyway, it will return early
        // Or if ads are disable, show ads anyway is true, it will NOT return early.
        if ( ! AreAdsEnabled && ! showAdAnyway) return;

        await LoadAdAsync
        (
              () => CrossMauiMTAdmob.Current.LoadRewarded(RewardedAdUnitId)
            , () => CrossMauiMTAdmob.Current.IsRewardedLoaded()
            , () => CrossMauiMTAdmob.Current.ShowRewarded()
            , timeoutInSeconds
        );
    }


}
