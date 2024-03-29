﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using TimeSince.Data;

namespace TimeSince.MVVM.BaseClasses;

public class BaseViewModel : INotifyPropertyChanged
{
    protected DataAccess? DataAccess { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
}
