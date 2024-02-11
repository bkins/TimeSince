using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    private RemoveAdsViewModel RemoveAdsViewModel { get; set; }

    public SettingsPage()
    {
        RemoveAdsViewModel = new RemoveAdsViewModel();

        InitializeComponent();

        BindingContext = this;
    }
}
