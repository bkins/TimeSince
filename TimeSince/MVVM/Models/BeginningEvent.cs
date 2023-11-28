using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using TimeSince.Avails;
using TimeSince.Avails.ColorHelpers;
using TimeSince.Avails.Extensions;
using TimeSince.MVVM.BaseClasses;
using TimeSince.MVVM.ViewModels;

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

    private Color _buttonTextColor = Color.FromArgb(ColorInfo.Black);
    [Ignore]
    public Color ButtonTextColor
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
        Title    = string.Empty;
        Date     = DateTime.Today;
        TimeSpan = DateTime.Now.TimeOfDay;
    }

    private void SetTimeSpan(TimeSpan timeSpan)
    {
        Time = timeSpan.ToShortForm();
    }

    #region INotifyPropertyChanged Properties and Methods

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this
                              , new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T                     field
                             , T                         value
                             , [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    #endregion
}
