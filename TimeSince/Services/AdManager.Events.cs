using Plugin.MauiMTAdmob.Extra;

namespace TimeSince.Services;

public partial class AdManager
{
    private const int TimesToRetryBannerOnFailure = 3;
    private       int _numberOfTimesTried;

    private void OnBannerAdLoaded(object? sender, EventArgs e)
    {
        // Handle the event when an ad is successfully loaded.
    }

    private void OnBannerAdFailedToLoad(object?      sender
                                      , MTEventArgs e)
    {
        //Maybe use Error_Codes:
        /* Google's documentation regarding Error_Codes:
           https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest#ERROR_CODE_APP_ID_MISSING

            public static final int ERROR_CODE_APP_ID_MISSING
            The ad request was not made due to a missing app ID.
            Constant Value: 8

            public static final int ERROR_CODE_INTERNAL_ERROR
            Something happened internally; for instance, an invalid response was received from the ad server.
            Constant Value: 0

            public static final int ERROR_CODE_INVALID_AD_STRING
            The ad string is invalid. For example, there is no request ID in the ad string.
            Constant Value: 11

            public static final int ERROR_CODE_INVALID_REQUEST
            The ad request was invalid; for instance, the ad unit ID was incorrect.
            Constant Value: 1

            public static final int ERROR_CODE_MEDIATION_NO_FILL
            The mediation adapter did not fill the ad request. If this error is returned from AdError.getCode(), check AdError.getCause() for the underlying cause.
            Constant Value: 9

            public static final int ERROR_CODE_NETWORK_ERROR
            The ad request was unsuccessful due to network connectivity.
            Constant Value: 2

            public static final int ERROR_CODE_NO_FILL
            The ad request was successful, but no ad was returned due to lack of ad inventory.
            Constant Value: 3

            public static final int ERROR_CODE_REQUEST_ID_MISMATCH
            The request ID in the ad string is not found.
            Constant Value: 10
         */
        if (e.ErrorCode == 3)
        {
            // If there is a "lack of ad inventory" retry, otherwise log the error
            while(_numberOfTimesTried < TimesToRetryBannerOnFailure)
            {
                InitializeBannerAd();
                _numberOfTimesTried++;
            }
        }
        else
        {
            LogErrors("load", e);
        }
    }
    private void CurrentOnUserEarnedReward(object?      sender
                                           , MTEventArgs e)
    {

    }

    private void CurrentOnRewardedFailedToShow(object?      sender
                                               , MTEventArgs e)
    {
        LogErrors("show", e);
    }

    private void CurrentOnRewardedFailedToLoad(object?      sender
                                               , MTEventArgs e)
    {
        LogErrors("load", e);
    }

    private void CurrentOnRewardedOpened(object?    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnRewardedLoaded(object?    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnRewardedImpression(object?    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnRewardedClosed(object?    sender
                                         , EventArgs e)
    {

    }

    private void CurrentOnRewardedClicked(object?    sender
                                          , EventArgs e)
    {

    }

    private void CurrentOnInterstitialOpened(object?    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnInterstitialLoaded(object?    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnInterstitialImpression(object?    sender
                                                 , EventArgs e)
    {

    }

    private void CurrentOnInterstitialClosed(object?    sender
                                             , EventArgs e)
    {

    }

    private void CurrentOnInterstitialClicked(object?    sender
                                              , EventArgs e)
    {

    }

    private void LogErrors(string activityWhenErrorOccurred, MTEventArgs e)
    {
        App.Logger.LogError($"Ad failed to {activityWhenErrorOccurred}: {e.ErrorCode} - {e.ErrorMessage}{Environment.NewLine}{e.ErrorDomain}"
                          , string.Empty
                          , string.Empty);
    }
}
