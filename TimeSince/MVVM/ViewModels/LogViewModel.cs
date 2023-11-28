using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeSince.Avails;
using TimeSince.MVVM.BaseClasses;
using TimeSince.MVVM.Models;

namespace TimeSince.MVVM.ViewModels;

public class LogViewModel : BaseViewModel
{
    private ObservableCollection<LogLine> _logEntries;
    public ObservableCollection<LogLine> LogEntries
    {
        get => _logEntries;
        set
        {
            _logEntries = value;
            OnPropertyChanged();
        }
    }

    private List<IGrouping<string, LogLine>> _groupedLogEntries;
    public List<IGrouping<string, LogLine>> GroupedLogEntries
    {
        get => _groupedLogEntries;
        set
        {
            _groupedLogEntries = value;
            OnPropertyChanged();
        }
    }

    private List<LogGroup> _groupedLogEntriesWithCount;
    public List<LogGroup> GroupedLogEntriesWithCount
    {
        get => _groupedLogEntriesWithCount;
        set
        {
            _groupedLogEntriesWithCount = value;
            OnPropertyChanged();
        }
    }

    public LogViewModel()
    {
        LoadLogEntries();
    }

    public void LoadLogEntries()
    {
        var json       = App.Logger.LogStringBuilder.ToString();
        var logList    = App.Logger.Deserialize(json);
        var logEntries = new ObservableCollection<LogLine>(logList);

        GroupedLogEntries = logEntries.OrderByDescending(log => log.Message)
                                      .ThenByDescending(log => log.TimestampDateTime)
                                      .GroupBy(log => log.Message)
                                      .Select(group => group)
                                      .ToList();
        //
        // GroupedLogEntriesWithCount = logEntries.OrderByDescending(log => log.Message)
        //                                        .ThenByDescending(log => log.TimestampDateTime)
        //                                        .GroupBy(log => log.Message)
        //                                        .Select(group => new LogGroup(group.Key
        //                                                                    , group.ToList()))
        //                                        .ToList();

        GroupedLogEntriesWithCount = logEntries.OrderByDescending(log => log.TimestampDateTime)
                                               .GroupBy(log => log.Message)
                                               .Select(group => new LogGroup(group.Key
                                                                           , group.ToList()))
                                               .ToList();

    }

    public static void ClearLogs()
    {
        App.Logger.Clear();
    }
}
public class LogGroup : List<LogLine>
{
    public string        Key      { get; set; }
    public string        LogCount { get; set; }
    public List<LogLine> Logs     { get; set; }

    public LogGroup(string key, List<LogLine> logLines) : base(logLines)
    {
        var splitKey = key?.Split('\n');

        Key      = splitKey?[0] ?? string.Empty;
        LogCount = $"Occurrences: {logLines.Count}";
        Logs     = logLines;
    }
}
