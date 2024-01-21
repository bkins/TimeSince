﻿using System.Collections.Specialized;
using Plugin.MauiMTAdmob.Extra;
using TimeSince.Avails;
using TimeSince.Avails.ColorHelpers;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.Models;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class MainPage //Events
{
    private void AddEventButton_OnClicked(object    sender
                                        , EventArgs e)
    {
        if ( ! App.AppServiceMethods.IsPhysicalDevice())
        {
            App.Logger.ToastMessage("Not a physical device.  These types of ads don't work that well here.");
        }
        else
        {
            App.AppServiceMethods.ShowInterstitialAdAsync();
        }

        TimeElapsedViewModel.AddNewEvent();
        ScrollToLastItemInListView();

        PreferencesDataStore.AwesomePersonScore += 10;
    }

    private void EventsListView_OnItemTapped(object              sender
                                           , ItemTappedEventArgs e)
    {
        if (e.Item is BeginningEvent selectedEvent)
        {
            TimeElapsedViewModel.Event = selectedEvent;
        }
    }

    private void SaveButton_OnClicked(object    sender
                                    , EventArgs e)
    {
        var eventToSave = TimeElapsedViewModel.GetEventFromSender(sender);

        if (eventToSave is null) return;

        TimeElapsedViewModel.Event = eventToSave;
        TimeElapsedViewModel.ExecuteSave();

        UiUtilities.TemporarilyChangeButtonText((Button)sender
                                              , "Success!"
                                              , 5
                                              , Dispatcher.CreateTimer());
        PreferencesDataStore.AwesomePersonScore += 0.1;
    }

    private void DeleteButton_OnClicked(object    sender
                                      , EventArgs e)
    {
        var eventToDelete = TimeElapsedViewModel.GetEventFromSender(sender);

        if (eventToDelete is null) return;

        TimeElapsedViewModel.Event = eventToDelete;
        TimeElapsedViewModel.ExecuteDelete();

        PreferencesDataStore.AwesomePersonScore += 0.1;
    }

    private void CloseButton_OnClicked(object    sender
                                     , EventArgs e)
    {
        Application.Current?.Quit();
    }

    private async void SettingsButton_OnClicked(object    sender
                                              , EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    private async void AboutPageButton_OnClick(object    sender
                                             , EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }

    private void SortButton_OnClicked(object    sender
                                    , EventArgs e)
    {
        SortPicker.IsVisible = ! SortPicker.IsVisible;
        //TimeElapsedViewModel.ToggleSort();
    }

    private void OnPageAppearing(object    sender
                               , EventArgs e)
    {
        ColorUtility.SetResourceColors();
        //SortPicker.TextColor          = ColorUtility.ChooseReadableTextColor(SortPicker.BackgroundColor);
        EventsListView.SeparatorColor = Color.FromArgb(ColorInfo.BlackAsHex); //ColorUtility.ChooseReadableTextColor(EventsListView.BackgroundColor);

        SetButtonsTextColor();
    }



    private void ElapsedTimeLabel_Tapped(object          sender
                                       , TappedEventArgs e)
    {
        TimeElapsedViewModel.IsLongTimeElapsedDisplayed = ! TimeElapsedViewModel.IsLongTimeElapsedDisplayed;
    }

    private void Events_OnCollectionChanged(object                           sender
                                           , NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add
         || e.NewItems == null) return;

        var newItem = e.NewItems[0];
        EventsListView.ScrollTo(newItem
                              , ScrollToPosition.End
                              , animated: true);
    }

    private void OnPickerSelectedIndexChanged_old(object sender, EventArgs e)
    {
        var picker        = (Picker)sender;
        var selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            TimeElapsedViewModel.SortEvents((SortOptions)selectedIndex);
        }
    }

    void OnSortPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker              = (Picker)sender;
        var selectedDescription = (string)picker.SelectedItem;

        if (selectedDescription.HasValue())
        {
            TimeElapsedViewModel.SortEvents(Utilities.GetEnumValueFromDescription<SortOptions>(selectedDescription));
            ScrollToFirstItemInListView();
        }
    }

    private void AddBanner_OnAdsClicked(object    sender
                                      , EventArgs e)
    {
        var s = "Ad clicked";
    }

    private void AddBanner_OnAdsClosed(object    sender
                                     , EventArgs e)
    {
        var s = "Ad closed";
    }

    private void AddBanner_OnAdsImpression(object    sender
                                         , EventArgs e)
    {
        var s = "Ad Impression";
    }

    private void AddBanner_OnAdsLeftApplication(object    sender
                                              , EventArgs e)
    {
        var s = "Ad Impression";
    }

    private void AddBanner_OnAdsFailedToLoad(object      sender
                                           , MTEventArgs e)
    {
        var s = "Ad FailedToLoad";
    }

    private void AddBanner_OnAdsLoaded(object    sender
                                     , EventArgs e)
    {
        var s = "Ad Loaded";
    }

    private void AddBanner_OnAdsOpened(object    sender
                                     , EventArgs e)
    {
        var s = "Ad AdsOpened";
    }

    private void PrivacyPolicyButton_OnClicked(object    sender
                                             , EventArgs e)
    {
        Launcher.OpenAsync(new Uri("https://benhop2.wixsite.com/bensapps/privacypolicy"));
    }
}
