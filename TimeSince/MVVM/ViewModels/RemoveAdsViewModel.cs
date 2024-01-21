using TimeSince.Data;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.ViewModels;

public class RemoveAdsViewModel : BaseViewModel
{
    public bool DoorIsUnlocked         { get; internal set; }
    public bool DoorReadOnlyIsUnlocked { get; set; }

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

    public RemoveAdsViewModel()
    {
        CheckLocks();
    }

    public void CheckLocks()
    {
        UnlockBackdoor();
        UnlockBackdoorReadOnly();
    }

    private async void UnlockBackdoor()
    {
        var doorKey = await App.AppServiceMethods
                               .GetDoorKay()
                               .ConfigureAwait(false);

        DoorIsUnlocked = doorKey == App.DoorKey
                      || doorKey == App.DoorKeyReadOnly;
    }

    private async void UnlockBackdoorReadOnly()
    {
        var doorKey = await App.AppServiceMethods
                               .GetDoorKay()
                               .ConfigureAwait(false);

        DoorReadOnlyIsUnlocked = doorKey == App.DoorKeyReadOnly;
    }
}
