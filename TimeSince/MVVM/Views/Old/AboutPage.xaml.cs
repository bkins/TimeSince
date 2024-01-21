using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class AboutPage_old : ContentPage
{
    private   AboutViewModel AboutViewModel { get; set; }
    protected View           AdView         { get; set; }

    public AboutPage_old()
    {
        InitializeComponent();

        SetButtonsTextColor();

        AboutViewModel = new AboutViewModel { CurrentAwesomeScore = "1" };

        BindingContext = AboutViewModel;

        AdView = App.AppServiceMethods.GetAdView();
        if (AdView is not null)
        {
            AdPlaceholder.Content = AdView;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        RefreshAdViewOnPage();
    }

    private void RefreshAdViewOnPage()
    {
        if (AboutViewModel.HasPurchasedToHideAds)
        {
            HideAds();

            return;
        }

        var adsAreEnabled        = App.AppServiceMethods.AreAdsEnabled();
        var adViewHasValue       = AdView is not null;
        var adViewIsInMainLayout = MainStackLayout.Children.Contains(AdView);

        if (adsAreEnabled && (! adViewHasValue || ! adViewIsInMainLayout))
        {
            ShowAds();

            return;
        }

        if (AdView is not null && ! adsAreEnabled && adViewIsInMainLayout)
        {
            HideAds();
        }

        if (App.AppServiceMethods.IsPhysicalDevice()) return;

        ShowInterstitialAdButton.IsEnabled = false;
        ShowRewardAdButton.IsEnabled       = false;

        App.Logger.ToastMessage("Ads cannot be shown on emulated devices.");
    }

    private void ShowAds()
    {
        if (AdView is null)
        {
            AdView           = App.AppServiceMethods.GetAdView();
            AdView.IsVisible = true;
        }

        if (!MainStackLayout.Children.Contains(AdView))
        {
            MainStackLayout.Children.Add(AdView);
        }

        // AdView           = AdView ?? App.AppServiceMethods.GetAdView();
        // AdView.IsVisible = true;
        // if ( ! MainStackLayout.Children.Contains(AdView))
        // {
        //     MainStackLayout.Children.Add(AdView);
        // }
    }

    private void HideAds()
    {
        if (AdView is null
         || ! MainStackLayout.Children.Contains(AdView)) return;

        AdView.IsVisible = false;
        MainStackLayout.Children.Remove(AdView);

        // if (AdView == null) return;
        //
        // AdView.IsVisible = false;
        // if (MainStackLayout.Children.Contains(AdView))
        // {
        //     MainStackLayout.Children.Remove(AdView);
        // }
    }

    private void ShowInterstitialAdButton_OnClicked(object    sender
                                                  , EventArgs e)
    {
        var originalButtonText = ShowInterstitialAdButton.Text;

        // Change the button text to indicate that the ad is loading
        ShowInterstitialAdButton.Text = "Ad is loading...";

        // Load the interstitial ad asynchronously
        App.AppServiceMethods.ShowInterstitialAdAsync();

        // Once the ad is shown, restore the original button text
        ShowInterstitialAdButton.Text = originalButtonText;

        AboutViewModel.CurrentAwesomeScore = "5";
    }

    //BENDO: Remove or implement
    private async void ShowInterstitialRewardAdButton_OnClicked(object    sender
                                                              , EventArgs e)
    {

        await App.AppServiceMethods.ShowInterstitialRewardAdAsync(10, true);
    }

    private async void ShowRewardAdButton_OnClicked(object    sender
                                                  , EventArgs e)
    {
        var originalButtonText = ShowRewardAdButton.Text;
        ShowRewardAdButton.Text = "Ad is loading...";

        await App.AppServiceMethods.ShowRewardAdAsync(10);

        ShowRewardAdButton.Text = originalButtonText;

        AboutViewModel.CurrentAwesomeScore = "10";
    }

    private void ResetAwesomeScoreButton_OnClicked(object    sender
                                                 , EventArgs e)
    {
        AboutViewModel.ResetAwesomeScore();
    }

    private void PrivacyPolicyButton_OnClicked(object    sender
                                             , EventArgs e)
    {
        Launcher.OpenAsync(new Uri("https://benhop2.wixsite.com/bensapps/privacypolicy"));
    }

    private async void TapGestureRecognizer_OnTapped(object          sender
                                                   , TappedEventArgs e)
    {
        try
        {
            var message = new EmailMessage("TimeSince: "
                                         , ""
                                         , EmailLabel.Text);
            await Email.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException notSupportedException)
        {
            App.Logger.LogError(notSupportedException);
        }
        catch (Exception ex)
        {
            App.Logger.LogError(ex);
        }
    }
    private void SetButtonsTextColor()
    {
        var buttonTextColor = ColorUtility.ChooseReadableTextColor(ColorUtility.GetColorFromResources(ResourceColors.Primary));
        PrivacyPolicyButton.TextColor      = buttonTextColor;
        ResetAwesomeScoreButton.TextColor  = buttonTextColor;
        ShowInterstitialAdButton.TextColor = buttonTextColor;
        ShowRewardAdButton.TextColor       = buttonTextColor;
    }
}
