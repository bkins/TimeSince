using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.Models;

public class LogEntryWrapper : BaseViewModel
{
    public string?  Key      { get; init; }
    public LogLine? Log      { get; init; }
    public bool     IsHeader { get; init; }
    public string?  LogCount { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    private bool _isCollapsed;
    public bool IsCollapsed
    {
        get => _isCollapsed;
        set
        {
            if (_isCollapsed == value) return;

            _isCollapsed = value;
            OnPropertyChanged();
        }
    }
}
