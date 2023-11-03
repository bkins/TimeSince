using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSince.Avails;
using TimeSince.Data;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class AboutPage : ContentPage
{
    private   AboutViewModel AboutViewModel { get; set; }
    protected View           AdView         { get; set; }
    public AboutPage()
    {
        InitializeComponent();

        AboutViewModel = new AboutViewModel();

        AboutViewModel.CurrentAwesomeScore = "1";

        BindingContext = AboutViewModel;
        AdView         = AdManager.Instance.GetAdView();
        if (AdView is not null)
        {
            MainStackLayout.Add(AdView);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        RefreshAdViewOnPage();

    }
    private void RefreshAdViewOnPage()
    {
        var adsAreEnabled        = AdManager.Instance.AreAdsEnabled;
        var adViewHasValue       = AdView is not null;
        var adViewIsInMainLayout = MainStackLayout.Children.Contains(AdView);

        if ( ! adViewHasValue
          && adsAreEnabled)
        {
            AdView           = AdManager.Instance.GetAdView();
            AdView.IsVisible = true;

            MainStackLayout.Add(AdView);

            return;
        }

        if (adViewHasValue
         && adsAreEnabled
         && ! adViewIsInMainLayout)
        {
            AdView.IsVisible = true;
            MainStackLayout.Add(AdView);

            return;
        }

        if (AdView is not null
         && ! adsAreEnabled
         && adViewIsInMainLayout)
        {
            AdView.IsVisible = false;
            MainStackLayout.Remove(AdView);
        }
    }
    private async void ShowInterstitialAdButton_OnClicked(object    sender
                                                        , EventArgs e)
    {
        var originalButtonText = ShowInterstitialAdButton.Text;

        // Change the button text to indicate that the ad is loading
        ShowInterstitialAdButton.Text = "Ad is loading...";

        // Load the interstitial ad asynchronously
        await AdManager.Instance.ShowInterstitialAdAsync(10, true);

        // Once the ad is shown, restore the original button text
        ShowInterstitialAdButton.Text = originalButtonText;

        AboutViewModel.CurrentAwesomeScore = "5";
    }

    private async void ShowInterstitialRewardAdButton_OnClicked(object    sender
                                                        , EventArgs e)
    {
        await AdManager.Instance.ShowInterstitialRewardAdAsync(10, true);
    }

    private async void ShowRewardAdButton_OnClicked(object    sender
                                            , EventArgs e)
    {
        var originalButtonText = ShowRewardAdButton.Text;
        ShowRewardAdButton.Text = "Ad is loading...";

        await AdManager.Instance.ShowRewardAdAsync(10, true);

        ShowRewardAdButton.Text = originalButtonText;

        AboutViewModel.CurrentAwesomeScore = "10";
    }

    private void ResetAwesomeScoreButton_OnClicked(object    sender
                                                 , EventArgs e)
    {
        AboutViewModel.ResetAwesomeScore();
    }
}
