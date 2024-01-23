using System.Text;
using TimeSince.Data;

namespace TimeSince.MVVM.Views;

public partial class AboutPage
{
    private void ShowInterstitialAdButton_OnClicked(object?    sender
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

    private void ResetAwesomeScoreButton_OnClicked(object?    sender
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
            var emailBody = BuildEmailBody();

            var message = new EmailMessage("TimeSince question/comment: "
                                         , emailBody
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

    private static string BuildEmailBody()
    {

        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("");
        bodyBuilder.AppendLine("");
        bodyBuilder.AppendLine(PreferencesDataStore.ErrorReportingId);
        bodyBuilder.AppendLine("This is a random unique identifier assigned to you.");
        bodyBuilder.AppendLine("This helps me identify any, and all, errors you personally had using this app.");
        bodyBuilder.AppendLine("This way I can get a greater context when addressing any issues you are having.");

        var emailBody = bodyBuilder.ToString();

        return emailBody;
    }
}
