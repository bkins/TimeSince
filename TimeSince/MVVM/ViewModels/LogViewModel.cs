﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeSince.MVVM.BaseClasses;
using TimeSince.MVVM.Models;

namespace TimeSince.MVVM.ViewModels;

public class LogViewModel : BaseViewModel
{
    public ICommand       DeleteCommand         { get; }
    public ICommand       ShowLogDetailsCommand { get; }
    public List<LogLine?> SelectedEntries       { get; set; } = new List<LogLine>();

    public ICommand DeleteSelectedEntriesCommand => new Command(DeleteSelectedEntries);

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
    public List<IGrouping<string?, LogLine>> GroupedLogEntries
    {
        get => _groupedLogEntries;
        set
        {
            _groupedLogEntries = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<LogEntryWrapper> _groupedLogEntriesWithCount;
    public ObservableCollection<LogEntryWrapper> GroupedLogEntriesWithCount
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
        _logEntries                 = [];
        _groupedLogEntries          = [];
        _groupedLogEntriesWithCount = [];

        DeleteCommand         = new Command<LogLine>(DeleteLog);
        ShowLogDetailsCommand = new Command<LogLine>(ShowLogDetails);
        LogEntries            = new ObservableCollection<LogLine>();

        LoadLogEntries();
    }

    private void LoadLogEntries()
    {
        var logEntries = ConvertRawJsonToObservableCollection();

        GroupedLogEntries = logEntries.OrderByDescending(log => log.Message)
                                      .ThenByDescending(log => log.TimestampDateTime)
                                      .GroupBy(log => log.Message)
                                      .Select(group => group)
                                      .ToList();
        LogEntries.Clear();

        foreach(var entry in logEntries)
        {
            LogEntries.Add(entry);
        }

        GroupedLogEntriesWithCount = [];
        RefreshGroupedLogEntries();
    }

    private static ObservableCollection<LogLine> ConvertRawJsonToObservableCollection()
    {
        var json    = App.Logger.LogStringBuilder.ToString();
        var logList = App.Logger.Deserialize(json);

        if (logList is null) return [];

        var logEntries = new ObservableCollection<LogLine>(logList);

        return logEntries;
    }

    public static void ClearLogs()
    {
        App.Logger.Clear();
    }

    public void DeleteLog(LogLine logLineToDelete)
    {
        LogEntries.Remove(logLineToDelete);
        RefreshGroupedLogEntries();

        OnPropertyChanged(nameof(GroupedLogEntriesWithCount));
    }

    private static void ShowLogDetails(LogLine selectedLog)
    {
        Console.WriteLine($"Tapped Log: {selectedLog.Message}");
    }

    private void DeleteSelectedEntries()
    {
        foreach (var entry in SelectedEntries.ToList().OfType<LogLine>())
        {
            App.Logger.DeleteLogEntry(entry);
        }

        App.Logger.RefreshLogsFromFile();

        OnPropertyChanged(nameof(GroupedLogEntriesWithCount));
    }

    private void RefreshGroupedLogEntries()
    {
        GroupedLogEntriesWithCount.Clear();

        GroupedLogEntries = LogEntries.OrderByDescending(log => log.Message)
                                      .ThenByDescending(log => log.TimestampDateTime)
                                      .GroupBy(log => log.Message)
                                      .Select(group => group)
                                      .ToList();

        foreach (var group in GroupedLogEntries)
        {
            GroupedLogEntriesWithCount.Add(new LogEntryWrapper
                                           {
                                               Key      = group.Key
                                             , LogCount = $"Occurrences: {group.Count()}"
                                             , IsHeader = true
                                           });

            foreach (var logLine in group)
            {
                GroupedLogEntriesWithCount.Add(new LogEntryWrapper
                                               {
                                                   Log      = logLine
                                                 , IsHeader = false
                                               });
            }
        }

        OnPropertyChanged(nameof(GroupedLogEntriesWithCount));
    }

    public void FilterEntriesByTodaysDate(bool showTodaysMessages)
    {
        GroupedLogEntriesWithCount.Clear();

        GroupedLogEntries = LogEntries.OrderByDescending(log => log.Message)
                                      .ThenByDescending(log => log.TimestampDateTime)
                                      .GroupBy(log => log.Message)
                                      .Select(group => group)
                                      .ToList();

        if (showTodaysMessages)
        {
            GroupedLogEntries = GroupedLogEntries.Where(logGroup => logGroup.FirstOrDefault()!
                                                                            .TimestampDateTime
                                                                            .Date
                                                                 == DateTime.Today)
                                                 .ToList();
        }

        foreach (var group in GroupedLogEntries)
        {
            GroupedLogEntriesWithCount.Add(new LogEntryWrapper
                                           {
                                               Key      = group.Key
                                             , LogCount = $"Occurrences: {group.Count()}"
                                             , IsHeader = true
                                           });

            foreach (var logLine in group)
            {
                GroupedLogEntriesWithCount.Add(new LogEntryWrapper
                                               {
                                                   Log      = logLine
                                                 , IsHeader = false
                                               });
            }
        }

        OnPropertyChanged(nameof(GroupedLogEntriesWithCount));
    }
}
