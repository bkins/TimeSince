using TimeSince.MVVM.Models;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views.Old;

public partial class MessageLog : ContentPage
{
    public LogViewModel LogEntryViewModel { get; set; }

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

    // private void OnAddButtonClicked(object    sender
    //                               , EventArgs e)
    // {
    //     try
    //     {
    //         throw new Exception("This is a test.  This is only a test");
    //     }
    //     catch (Exception exception)
    //     {
    //         App.Logger.LogError(exception, "Test message thrown from MessageLog page.");
    //     }
    //
    //     LogEntryViewModel.LoadLogEntries();
    // }
}
