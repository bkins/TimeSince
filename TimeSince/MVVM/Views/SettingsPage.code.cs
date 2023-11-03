using Syncfusion.Maui.Picker;
using TimeSince.MVVM.Models;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    protected RemoveAdsViewModel RemoveAdsViewModel { get; set; }

    private ColorName _selectedColor;
    public ColorName SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor == value) return;

            _selectedColor = value;
            OnPropertyChanged();
        }
    }

    public SettingsPage()
    {
        RemoveAdsViewModel = new RemoveAdsViewModel();

        InitializeComponent();


        BindingContext = this;
    }

    public void FindCurrentlySetColor()
    {
        //TODO when the settingsPage is opened, find the currently set color for each Primary, Secondary, and Tertiary
        //colors in the appropriate picker
    }

}
