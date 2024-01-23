using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace TimeSince.MVVM.BaseClasses;

public class BaseModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    #region INotifyPropertyChanged Properties and Methods

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this
                              , new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T                      field
                             , T                          value
                             , [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    #endregion
}
