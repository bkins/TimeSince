using TimeSince.MVVM.Models;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public partial class MessageLog : ContentPage
{
    private LogViewModel LogEntryViewModel { get; set; }
    private bool         IsSelected        { get; set; }

    public MessageLog()
    {
        InitializeComponent();

        LogEntryViewModel = new LogViewModel();

        BindingContext = LogEntryViewModel;

#if DEBUG

#endif
    }
    private T FindParent<T>(Element element) where T : Element
    {
        var parent = element.Parent;
        while (parent != null)
        {
            if (parent is T typedParent)
            {
                return typedParent;
            }
            parent = parent.Parent;
        }
        return null;
    }

    private async void OnLogListViewOnItemTapped(object          sender
                                               , TappedEventArgs tappedEventArgs)
    {
        if (sender is not Frame { BindingContext: LogEntryWrapper logEntryWrapper }
         || logEntryWrapper.IsHeader)
        {
            return;
        }

        var selectedLog = logEntryWrapper.Log;
        var userChoseOk = await DisplayAlert("Details"
                                          , $"Message: {selectedLog.Message}\nExtra Details: {selectedLog.ExtraDetails}"
                                          , "OK"
                                          , "Copy").ConfigureAwait(false);
        if (userChoseOk) return;

        //Copy log to clipboard
        var messageToCopy = selectedLog.ToString();
        await Clipboard.SetTextAsync(messageToCopy);
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

    private void DeleteSelectedToolbarItem_OnClicked(object    sender
                                                   , EventArgs e)
    {
        if (BindingContext is not LogViewModel viewModel) return;

        viewModel.SelectedEntries = viewModel.GroupedLogEntriesWithCount
                                             .Where(entry => entry.IsSelected)
                                             .Select(entry => entry.Log)
                                             .ToList();

        viewModel.DeleteSelectedEntriesCommand.Execute(null);

        LogListView.ItemsSource = viewModel.GroupedLogEntriesWithCount;

        LogEntryViewModel = new LogViewModel();

        BindingContext = LogEntryViewModel;
    }

    private void TapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
    {
        var group = sender as Label;

        foreach (var logEntry in LogEntryViewModel.GroupedLogEntriesWithCount)
        {
            if (logEntry.IsHeader
                && logEntry.Key == group?.Text)
            {
                logEntry.IsCollapsed = ! logEntry.IsCollapsed;
            }
        }
    }
}
