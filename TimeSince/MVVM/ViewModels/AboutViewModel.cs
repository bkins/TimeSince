using System.Globalization;
using TimeSince.Data;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.ViewModels;

public class AboutViewModel : BaseViewModel
{
    public  string CurrentVersion { get; set; }
    public  string CurrentBuild   { get; set; }
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

    public AboutViewModel ()
    {
        CurrentVersion                = VersionTracking.CurrentVersion;
        CurrentBuild                  = GetBuildName(VersionTracking.CurrentBuild);

        //CurrentAwesomeScore = PreferencesDataStore.AwesomePersonScore.ToString("N2");
    }

    private string GetBuildName(string buildNumber)
    {
        return buildNumber switch
               {
                   "1" => "Alpha",
                   "2" => "Beta",
                   "3" => "RC",
                   "4" => "Prod",
                   _ => "Unknown"
               };
    }

    public void ResetAwesomeScore()
    {
        PreferencesDataStore.AwesomePersonScore = 0;
        CurrentAwesomeScore                     = "0";
    }
}
