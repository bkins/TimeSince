using System.Globalization;
using TimeSince.Data;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.ViewModels;

public class AboutViewModel : BaseViewModel
{
    public string CurrentVersion { get; set; }
    public string CurrentBuild   { get; set; }
    public string CurrentMode    { get; set; }


    private string _currentAwesomeScore;
    public string CurrentAwesomeScore
    {
        get => _currentAwesomeScore;
        set
        {
            if (_currentAwesomeScore == value) return;

            PreferencesDataStore.AwesomePersonScore += double.Parse(value);

            _currentAwesomeScore = PreferencesDataStore.AwesomePersonScore.ToString("N2");

            OnPropertyChanged();
        }
    }

    private bool _isPrivacyPolicyAccepted;
    public bool IsPrivacyPolicyAccepted
    {
        get => _isPrivacyPolicyAccepted;
        set
        {
            if (_isPrivacyPolicyAccepted == value) return;

            PreferencesDataStore.HideStartupMessage = value;

            _isPrivacyPolicyAccepted = PreferencesDataStore.HideStartupMessage;

            OnPropertyChanged();
        }
    }

    private bool _hasPurchasedToHideAds;

    public bool HasPurchasedToHideAds
    {
        get => _hasPurchasedToHideAds;
        set
        {
            if (_hasPurchasedToHideAds == value) return;

            PreferencesDataStore.PaidToTurnOffAds = value;

            _hasPurchasedToHideAds = PreferencesDataStore.PaidToTurnOffAds;

            OnPropertyChanged();
        }
    }

    public AboutViewModel ()
    {
        // CurrentVersion = VersionTracking.CurrentVersion;
        // CurrentBuild   = GetBuildName(VersionTracking.CurrentBuild);

        CurrentVersion          = App.AppServiceMethods.AppInfo.CurrentVersion;
        CurrentBuild            = App.AppServiceMethods.AppInfo.CurrentBuild;
        CurrentMode             = App.AppServiceMethods.GetMode();

        IsPrivacyPolicyAccepted = PreferencesDataStore.HideStartupMessage;
        HasPurchasedToHideAds   = PreferencesDataStore.PaidToTurnOffAds;

        //CurrentAwesomeScore = PreferencesDataStore.AwesomePersonScore.ToString("N2");
    }

    private string GetBuildName(string buildNumber)
    {
        return buildNumber switch
               {
                   "1" => "Alpha"
                 , "2" => "Beta"
                 , "3" => "RC"
                 , "4" => "Prod"
                 , _ => "Unknown"
               };
    }

    public void ResetAwesomeScore()
    {
        PreferencesDataStore.AwesomePersonScore = 0;
        CurrentAwesomeScore                     = "0";
    }
}
