using Syncfusion.Maui.Picker;
using TimeSince.Avails.Attributes;
using TimeSince.Avails.ColorHelpers;
using TimeSince.Data;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage
{

    private void PrimaryColorPickerOnSelectionChanged(object                          sender
                                             , PickerSelectionChangedEventArgs e)
    {
        //BENDO: Investigate why Primary is handled differently than Secondary and Tertiary.

        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;
        var color         = ColorUtility.ColorNames[selectedIndex];

        PrimaryColorPicker.BackgroundColor = color.Color;
        PrimaryColorPicker.FooterView
                          .TextStyle = new PickerTextStyle
                                                  {
                                                      TextColor = ColorUtility.ChooseReadableTextColor(PrimaryColorPicker.BackgroundColor)
                                                  };
    }

    private void SecondaryColorPickerOnSelectionChanged(object                          sender
                                                      , PickerSelectionChangedEventArgs e)
    {
        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;

        var color     = ColorUtility.ColorNames[selectedIndex];
        var mauiColor = ColorUtility.ConvertSystemColorNameToMauiColor(color.Name, ColorInfo.LavenderMist);

        SecondaryColorPicker.BackgroundColor = mauiColor;
        SecondaryColorPicker.FooterView
                            .TextStyle = new PickerTextStyle
                                         {
                                             TextColor = ColorUtility.ChooseReadableTextColor(SecondaryColorPicker.BackgroundColor)
                                         };
    }

    private void TertiaryColorPickerOnSelectionChanged(object                          sender
                                                     , PickerSelectionChangedEventArgs e)
    {
        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;

        var color     = ColorUtility.ColorNames[selectedIndex];
        var mauiColor = ColorUtility.ConvertSystemColorNameToMauiColor(color.Name, ColorInfo.MidnightIndigo);
        var didSetColor = ColorUtility.UpdateColorResource(ResourceColors.Tertiary
                                                         , mauiColor);

        TertiaryColorPicker.BackgroundColor = mauiColor;
        TertiaryColorPicker.FooterView
                           .TextStyle = new PickerTextStyle
                                        {
                                            TextColor = ColorUtility.ChooseReadableTextColor(TertiaryColorPicker.BackgroundColor)
                                        };
    }

    private void PrimaryColorPickerOkButtonClicked(object    sender
                                                 , EventArgs e)
    {
        var didSetColor = ColorUtility.UpdateColorResource(ResourceColors.Primary
                                                       , PrimaryColorPicker.BackgroundColor);

        if ( ! didSetColor) return;

        TogglePrimaryControlsVisibilities();
    }

    private void SecondaryColorPickerOkButtonClicked(object    sender
                                                   , EventArgs e)
    {
        var didSetColor = ColorUtility.UpdateColorResource(ResourceColors.Secondary
                                                       , SecondaryColorPicker.BackgroundColor);

        if ( ! didSetColor) return;

        ToggleSecondaryControlsVisibilities();
    }

    private void TertiaryColorPickerOkButtonClicked(object    sender
                                                  , EventArgs e)
    {
        var didSetColor = ColorUtility.UpdateColorResource(ResourceColors.Tertiary
                                                       , TertiaryColorPicker.BackgroundColor);

        if ( ! didSetColor) return;

        ToggleTertiaryControlsVisibilities();
    }

    private void PrimarySearchEditorOnTextChanged(object               sender
                                                , TextChangedEventArgs e)
    {
        PrimaryColorPicker.Columns[0]
                          .SelectedIndex = ColorUtility.GetIndexFromPartialName(PrimarySearchEditor.Text);
    }

    private void SecondarySearchEditorOnTextChanged(object               sender
                                                  , TextChangedEventArgs e)
    {
        SecondaryColorPicker.Columns[0]
                            .SelectedIndex = ColorUtility.GetIndexFromPartialName(SecondarySearchEditor.Text);
    }

    private void TertiarySearchEditorOnTextChanged(object               sender
                                                                               , TextChangedEventArgs e)
    {
        TertiaryColorPicker.Columns[0]
                           .SelectedIndex = ColorUtility.GetIndexFromPartialName(TertiarySearchEditor.Text);
    }

    private void PrimaryButtonOnClicked(object    sender
                                      , EventArgs e)
    {
        TogglePrimaryControlsVisibilities();
    }

    private void TogglePrimaryControlsVisibilities()
    {
        PrimarySearchEditor.IsVisible = ! PrimarySearchEditor.IsVisible;
        PrimaryColorPicker.IsVisible  = ! PrimaryColorPicker.IsVisible;

        PrimaryButton.IsVisible = ! PrimaryButton.IsVisible;
    }

    private void ToggleSecondaryControlsVisibilities()
    {
        SecondarySearchEditor.IsVisible = ! SecondarySearchEditor.IsVisible;
        SecondaryColorPicker.IsVisible  = ! SecondaryColorPicker.IsVisible;

        SecondaryButton.IsVisible = ! SecondaryButton.IsVisible;
    }

    private void ToggleTertiaryControlsVisibilities()
    {
        TertiarySearchEditor.IsVisible = ! TertiarySearchEditor.IsVisible;
        TertiaryColorPicker.IsVisible  = ! TertiaryColorPicker.IsVisible;

        TertiaryButton.IsVisible = ! TertiaryButton.IsVisible;
    }
    private void SecondaryButtonOnClicked(object    sender
                                        , EventArgs e)
    {
        ToggleSecondaryControlsVisibilities();
    }

    private void TertiaryButtonOnClicked(object    sender
                                       , EventArgs e)
    {
        TertiarySearchEditor.IsVisible = true;
        TertiaryColorPicker.IsVisible  = true;

        TertiaryButton.IsVisible = false;
    }

    private void PrimaryColorPickerCancelButtonClicked(object    sender
                                                     , EventArgs e)
    {
        TogglePrimaryControlsVisibilities();
    }

    private void SecondaryColorPickerCancelButtonClicked(object    sender
                                                       , EventArgs e)
    {
        ToggleSecondaryControlsVisibilities();
    }

    private void TertiaryColorPickerCancelButtonClicked(object    sender
                                                      , EventArgs e)
    {
        ToggleTertiaryControlsVisibilities();
    }

    private void ResetColorsButtonOnClicked(object    sender
                                          , EventArgs e)
    {
        PreferencesDataStore.ClearColors();
        ColorUtility.SetDefaultResourceColors();
    }

    [UnderConstruction("Need to implement way to make a purchase to remove ads")]
    private void RemoveAdsSwitchOnToggled(object           sender
                                        , ToggledEventArgs e)
    {
        DisplayAlert("Under Construction"
                   , "This feature is still under construction."
                   , "OK");

        // 1. Make purchase to remove ads
        App.AppServiceMethods.MakePurchase();
        // OR
        App.AppServiceMethods.PurchaseItem();

        // 2. Verify purchase was made

        // 3. Turn off ads in app
        RemoveAdsViewModel.PaidForAdsToBeRemoved = RemoveAdsSwitch.IsToggled;

        if (AdView is not null)
        {
            AdView.IsVisible = RemoveAdsSwitch.IsToggled;
        }
    }


}
