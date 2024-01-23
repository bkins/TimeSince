using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using TimeSince.Avails.ColorHelpers;
using TimeSince.Avails.Extensions;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.Models;

[Table($"{nameof(BeginningEvent)}s")]
public class BeginningEvent : BaseModel, INotifyPropertyChanged
{
    public string   Title { get; set; }
    public DateTime Date  { get; set; }
    public string   Time  { get; set; }

    public TimeSpan TimeSpan
    {
        get => Time.ToTimeSpan();
        set => SetTimeSpan(value);
    }

    public TimeSpan TimeElapsed { get; set; }

    private string _timeElapsedForDisplay;
    [Ignore]
    public string TimeElapsedForDisplay
    {
        get => _timeElapsedForDisplay;
        set
        {
            if (_timeElapsedForDisplay == value) return;

            _timeElapsedForDisplay = value;
            OnPropertyChanged();
        }
    }

    private Color? _buttonTextColor = Color.FromArgb(ColorInfo.Black);
    [Ignore]
    public Color? ButtonTextColor
    {
        get => _buttonTextColor;
        set
        {
            if (Equals(_buttonTextColor, value)) return;

            _buttonTextColor = value;
            OnPropertyChanged();
        }
    }

    public BeginningEvent()
    {
        Title                  = string.Empty;
        Date                   = DateTime.Today;
        TimeSpan               = DateTime.Now.TimeOfDay;
        Time                   = string.Empty;
        _timeElapsedForDisplay = string.Empty;
    }

    private void SetTimeSpan(TimeSpan timeSpan)
    {
        Time = timeSpan.ToShortForm();
    }


}
