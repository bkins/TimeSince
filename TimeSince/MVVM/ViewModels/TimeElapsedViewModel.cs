using System.Collections.ObjectModel;
using System.ComponentModel;
using TimeSince.Avails;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.BaseClasses;
using TimeSince.MVVM.Models;

namespace TimeSince.MVVM.ViewModels;

public class TimeElapsedViewModel : BaseViewModel
{
    public BeginningEvent Event { get; set; }

    private ObservableCollection<BeginningEvent> _events;
    public ObservableCollection<BeginningEvent> Events
    {
        get => _events;
        set
        {
            if (_events == value) return;
            _events = value;
            OnPropertyChanged();
        }
    }

    private SortOptions _currentSortOption;

    /// <summary>
    /// Property to store the current sorting option
    /// </summary>
    public SortOptions CurrentSortOption
    {
        get => _currentSortOption;
        set
        {
            if (_currentSortOption == value) return;
            _currentSortOption = value;
            OnPropertyChanged();

        }
    }

    public List<string> SortOptionsList { get; } = Enum.GetValues(typeof(SortOptions))
                                                       .Cast<SortOptions>()
                                                       .Select(e => e.GetDescription())
                                                       .ToList();

    public bool IsLongTimeElapsedDisplayed = false;

    public  Command SaveCommand       { get; }
    public  Command DeleteCommand     { get; }

    public  Command ToggleSortCommand { get; }

    public TimeElapsedViewModel()
    {
        DataAccess = new DataAccess(App.Database);
        Events     = DataAccess.GetObservableCollection<BeginningEvent>();

        SaveCommand       = new Command(ExecuteSave);
        DeleteCommand     = new Command(ExecuteDelete);
        ToggleSortCommand = new Command(ToggleSort);

        StartUpdatingElapsedTime();

        CurrentSortOption = SortOptions.DateTime;
    }

    public void ExecuteSave()
    {
        if (Event.Id == 0)
        {
            DataAccess.Insert(Event);
        }
        else
        {
            DataAccess.Update(Event);
            UpdateExistingEvent();
        }
    }

    public void ExecuteDelete()
    {
        if (Event.Id > 0)
        {
            DataAccess.Delete(Event);
        }
        Events.Remove(Event);
    }

    private async void StartUpdatingElapsedTime()
    {
        while (true)
        {
            foreach (var beginningEvent in Events)
            {
                beginningEvent.TimeElapsed           = DateTime.Now - GetEventDateTime(beginningEvent);
                beginningEvent.TimeElapsedForDisplay = IsLongTimeElapsedDisplayed
                                                            ? GetLongTimeElapsed(beginningEvent)
                                                            : GetTimeElapsed(beginningEvent);
            }

            OnPropertyChanged(nameof(Events));

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static string GetTimeElapsed(BeginningEvent beginningEvent)
    {
        try
        {
            var elapsedTime = beginningEvent.TimeElapsed;

            var hours   = elapsedTime.Hours.ToString().PadLeft(2, '0');
            var minutes = elapsedTime.Minutes.ToString().PadLeft(2, '0');
            var seconds = elapsedTime.Seconds.ToString().PadLeft(2, '0');
            var s       = elapsedTime.Days != 1 ? "s" : string.Empty;

            return $"{elapsedTime.Days} day{s}, {hours}:{minutes}:{seconds}";
        }
        catch (Exception e)
        {
            App.Logger.LogError(e);

            return "Please set a date and time for the event";
        }
    }

    private static string GetLongTimeElapsed(BeginningEvent beginningEvent)
    {
        try
        {
            var elapsedTime = beginningEvent.TimeElapsed;

            var years   = elapsedTime.Days / 365;
            var months  = elapsedTime.Days % 365 / 30;
            var days    = elapsedTime.Days % 30;
            var hours   = elapsedTime.Hours.ToString().PadLeft(2, '0');
            var minutes = elapsedTime.Minutes.ToString().PadLeft(2, '0');
            var seconds = elapsedTime.Seconds.ToString().PadLeft(2, '0');

            var yearString  = PluralizeVariable("year", years);
            var monthString = PluralizeVariable("month", months);
            var dayString   = PluralizeVariable("day", days);

            return $"{yearString}{monthString}{dayString}{hours}:{minutes}:{seconds}";
        }
        catch (Exception e)
        {
            App.Logger.LogError(e);

            return "Please set a date and time for the event";
        }
    }

    private static string PluralizeVariable(string variable, int variableValue)
    {
        return variableValue > 0
                    ? $"{variableValue} {variable}{(variableValue != 1 ? "s" : string.Empty)}, "
                    : string.Empty;
    }

    private void UpdateExistingEvent()
    {
        foreach (var beginningEvent in Events.Where(beginningEvent => beginningEvent.Id == Event.Id))
        {
            beginningEvent.Date     = Event.Date;
            beginningEvent.TimeSpan = Event.TimeSpan;
            beginningEvent.Time     = Event.Time;
            beginningEvent.Title    = Event.Title;
        }
    }

    private DateTime GetEventDateTime(BeginningEvent beginningEvent)
    {
        if (beginningEvent is null) return DateTime.Now;

        return new DateTime(beginningEvent.Date.Year
                          , beginningEvent.Date.Month
                          , beginningEvent.Date.Day
                          , beginningEvent.TimeSpan.Hours
                          , beginningEvent.TimeSpan.Minutes
                          , beginningEvent.TimeSpan.Seconds);
    }

    public BeginningEvent GetEventFromSender(object sender)
    {
        var saveButton = (Button)sender;

        return saveButton.BindingContext as BeginningEvent;
    }

    public void AddNewEvent()
    {
        Events.Add(new BeginningEvent());
    }

    public void SortEvents(SortOptions sortBy)
    {
        PreferencesDataStore.LastSortOption = (int)sortBy;

        switch (sortBy)
        {
            case SortOptions.None:
                Events = DataAccess.GetObservableCollection<BeginningEvent>();
                break;

            case SortOptions.DateTime:
                SortEventsByDateAndTime();
                break;

            case SortOptions.DateTimeDesc:
                SortEventsByDateAndTime(descending: true);
                break;

            case SortOptions.Title:
                SortEventsByTitle();
                break;

            case SortOptions.TitleDesc:
                SortEventsByTitle(descending: true);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(sortBy)
                                                    , sortBy
                                                    , null);
        }
    }

    private void SortEventsByDateAndTime(bool descending = false)
    {

        var sortedEventsByDateTime = descending
                                        ? Events.OrderByDescending(beginningEvent => beginningEvent.Date)
                                                .ThenByDescending(beginningEvent => beginningEvent.TimeSpan).ToList()
                                        : Events.OrderBy(beginningEvent => beginningEvent.Date)
                                                .ThenBy(beginningEvent => beginningEvent.TimeSpan).ToList();

        RebuildEvents(sortedEventsByDateTime);
    }

    private void SortEventsByTitle(bool descending = false)
    {
        var sortedEventsByTitle = descending
                                    ? Events.OrderByDescending(beginningEvent => beginningEvent.Title).ToList()
                                    : Events.OrderBy(beginningEvent => beginningEvent.Title).ToList();

        RebuildEvents(sortedEventsByTitle);
    }

    private void RebuildEvents(List<BeginningEvent> sortedEventsByTitle)
    {
        Events.Clear();
        foreach (var beginningEvent in sortedEventsByTitle)
        {
            Events.Add(beginningEvent);
        }
    }

    private void ToggleSort()
    {
        CurrentSortOption = CurrentSortOption == SortOptions.DateTime
                                ? SortOptions.Title
                                : SortOptions.DateTime;

        SortEvents(CurrentSortOption);
    }
}

public enum SortOptions
{
    [Description("None")]
    None
  , [Description("Date & Time (oldest to newest)")]
    DateTime
  , [Description("Date & Time (newest to oldest)")]
    DateTimeDesc
  , [Description("Title (A to Z)")]
    Title
  , [Description("Title (Z to A)")]
    TitleDesc
}
