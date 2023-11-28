using TimeSince.MVVM.Models;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class MessageLog : ContentPage
{
    private LogViewModel LogEntryViewModel { get; set; }
    public MessageLog()
    {
        InitializeComponent();

        LogEntryViewModel = new LogViewModel();

        BindingContext = LogEntryViewModel;
    }

    private async void OnLogListViewOnItemTapped(object              sender
                                               , ItemTappedEventArgs args)
    {
        if (args.Item is LogLine selectedLog)
        {
            var userChoice = await DisplayAlert("Details"
                                              , $"Message: {selectedLog.Message}\nExtra Details: {selectedLog.ExtraDetails}"
                                              , "OK"
                                              , "Copy")
                .ConfigureAwait(false);
            if (! userChoice)
            {
                var messageToCopy = selectedLog.ToString();
                await Clipboard.SetTextAsync(messageToCopy);
            }
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (sender != null) ((ListView)sender).SelectedItem = null; // Deselect the item
        });

    }

    private async void ClearToolbarItem_OnClicked(object    sender
                                                , EventArgs e)
    {
        var deleteLogs = await DisplayAlert("Clear All Log Entries?"
                                          , "This will clear out all log entries.  This action cannot be undone.  Would you like to continue?"
                                          , "Yes, Delete Logs"
                                          , "No, Leave the logs");
        if (deleteLogs)
        {
            LogViewModel.ClearLogs();
            await Navigation.PopAsync();
        }
    }
}
