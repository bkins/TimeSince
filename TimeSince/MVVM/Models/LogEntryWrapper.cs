using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.Models;

public class LogEntryWrapper : BaseViewModel
{
    public  string  Key      { get; set; }
    public  LogLine Log      { get; set; }
    public  bool    IsHeader { get; set; }
    public  string  LogCount { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;

            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    private bool _isCollapsed;
    public bool IsCollapsed
    {
        get => _isCollapsed;
        set
        {
            if (_isCollapsed != value)
            {
                _isCollapsed = value;
                OnPropertyChanged(nameof(IsCollapsed));
            }
        }
    }

}
