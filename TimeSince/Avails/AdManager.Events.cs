using Plugin.MauiMTAdmob.Extra;

namespace TimeSince.Avails;

public partial class AdManager
{
    private void OnBannerAdLoaded(object sender, EventArgs e)
    {
        // Handle the event when an ad is successfully loaded.
    }

    private void OnBannerAdFailedToLoad(object      sender
                                      , MTEventArgs e)
    {
        LogErrors("load", e);
    }
    private void CurrentOnOnUserEarnedReward(object      sender
                                           , MTEventArgs e)
    {

    }

    private void CurrentOnOnRewardedFailedToShow(object      sender
                                               , MTEventArgs e)
    {
        LogErrors("show", e);
    }

    private void CurrentOnOnRewardedFailedToLoad(object      sender
                                               , MTEventArgs e)
    {
        LogErrors("load", e);
    }

    private void CurrentOnOnRewardedOpened(object    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnOnRewardedLoaded(object    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnOnRewardedImpression(object    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnOnRewardedClosed(object    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnOnRewardedClicked(object    sender
                                          , EventArgs e)
    {

    }

    private void CurrentOnOnInterstitialOpened(object    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnOnInterstitialLoaded(object    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnOnInterstitialImpression(object    sender
                                                 , EventArgs e)
    {

    }

    private void CurrentOnOnInterstitialClosed(object    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnOnInterstitialClicked(object    sender
                                              , EventArgs e)
    {

    }

    private void LogErrors(string activityWhenErrorOccurred, MTEventArgs e)
    {
        Console.WriteLine($"Ad failed to {activityWhenErrorOccurred}: {e.ErrorCode} - {e.ErrorMessage}{Environment.NewLine}{e.ErrorDomain}");
    }
}
