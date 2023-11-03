using System.Collections.ObjectModel;
using TimeSince.MVVM.Models;

namespace TimeSince.Data;

public class DataAccess
{
    private AbstractDataStore DataStore { get; set; }

    public DataAccess(AbstractDataStore dataStore)
    {
        DataStore = dataStore;
    }

    public void Insert(BeginningEvent beginningEvent)
    {
        DataStore.InsertEvent(beginningEvent);
    }

    public void Update(BeginningEvent beginningEvent)
    {
        DataStore.UpdateEvent(beginningEvent);
    }

    public BeginningEvent GetBeginningEvent(int id)
    {
        return DataStore.GetBeginningEvent(id);
    }

    public ObservableCollection<T> GetObservableCollection<T>() where T : new()
    {
        return DataStore.GetObservableCollection<T>();
    }

    public void Delete(BeginningEvent beginningEvent)
    {
        DataStore.DeleteEvent(beginningEvent);
    }
}
