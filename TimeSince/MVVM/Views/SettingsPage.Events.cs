using Syncfusion.Maui.Picker;
using TimeSince.Avails;
using TimeSince.Data;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage
{

    private void PrimaryColorPickerOnSelectionChanged(object                          sender
                                             , PickerSelectionChangedEventArgs e)
    {
        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;
        var color         = ColorUtility.ColorNames[selectedIndex];

        PrimaryColorPicker.BackgroundColor = color.Color;
    }

    private void SecondaryColorPickerOnSelectionChanged(object                          sender
                                                      , PickerSelectionChangedEventArgs e)
    {
        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;

        var color     = ColorUtility.ColorNames[selectedIndex];
        var mauiColor = ColorUtility.ConvertSystemColorNameToMauiColor(color.Name);

        SecondaryColorPicker.BackgroundColor = mauiColor;
    }

    private void TertiaryColorPickerOnSelectionChanged(object                          sender
                                                     , PickerSelectionChangedEventArgs e)
    {
        if (e.NewValue < 0
         || e.NewValue >= ColorUtility.ColorNames.Count) return;

        var selectedIndex = e.NewValue;

        var color     = ColorUtility.ColorNames[selectedIndex];
        var mauiColor = ColorUtility.ConvertSystemColorNameToMauiColor(color.Name);
        var didSetColor = ColorUtility.UpdateColorResource(ResourceColors.Tertiary
                                                       , mauiColor);

        TertiaryColorPicker.BackgroundColor = mauiColor;
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

    private void RemoveAdsCheckBoxOnCheckedChanged(object                  sender
                                                 , CheckedChangedEventArgs e)
    {
        RemoveAdsViewModel.PaidForAdsToBeRemoved = RemoveAdsCheckBox.IsChecked;

        if (AdView is not null)
        {
            AdView.IsVisible = RemoveAdsCheckBox.IsChecked;
        }

        if (RemoveAdsCheckBox.IsChecked)
        {

        }
    }

}
