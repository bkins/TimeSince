using TimeSince.Data;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.ViewModels;

public class RemoveAdsViewModel : BaseViewModel
{
    private bool _paidForAdsToBeRemoved = PreferencesDataStore.PaidToTurnOffAds;

    public bool PaidForAdsToBeRemoved
    {
        get => _paidForAdsToBeRemoved;
        set
        {
            _paidForAdsToBeRemoved = value;

            PreferencesDataStore.PaidToTurnOffAds = _paidForAdsToBeRemoved;
        }
    }
}
