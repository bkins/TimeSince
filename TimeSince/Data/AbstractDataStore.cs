using System.Collections.ObjectModel;
using TimeSince.MVVM.Models;

namespace TimeSince.Data;

public abstract class AbstractDataStore
{
    public abstract void UpdateEvent(BeginningEvent beginningEvent);
    public abstract void InsertEvent(BeginningEvent beginningEvent);
    public abstract void DeleteEvent(BeginningEvent beginningEvent);

    public abstract BeginningEvent? GetBeginningEvent(int? id = null);
    public abstract ObservableCollection<BeginningEvent> GetBeginningEvents();
    public abstract ObservableCollection<T> GetObservableCollection<T>() where T : new();
}
