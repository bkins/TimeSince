using System.Collections.ObjectModel;
using SQLite;
using SQLiteNetExtensions.Extensions;
using TimeSince.Avails.Exceptions;
using TimeSince.MVVM.Models;

namespace TimeSince.Data;

public class SqliteDatabase : AbstractDataStore
{
    private readonly SQLiteConnection _database;

    public SqliteDatabase(string dbPath)
    {
        try
        {
            _database = new SQLiteConnection(dbPath);

            _database.CreateTable<BeginningEvent>();
        }
        catch (Exception e)
        {
            App.Logger.LogError(e);
        }
    }

    public override void UpdateEvent(BeginningEvent beginningEvent)
    {
        int rowsAffected;

        try
        {
            rowsAffected = _database.Update(beginningEvent);
        }
        catch (Exception e)
        {
            throw new DatabaseCommitFailedException("Error occurred while saving the record", e);
        }

        ValidateAffectedRows(rowsAffected);
    }
    public override void InsertEvent(BeginningEvent beginningEvent)
    {
        int rowsAffected;

        try
        {
            rowsAffected = _database.Insert(beginningEvent);

        }
        catch (Exception e)
        {
            throw new DatabaseCommitFailedException("Error occurred while saving the record", e);
        }

        ValidateAffectedRows(rowsAffected);
    }

    public override void DeleteEvent(BeginningEvent beginningEvent)
    {
        int rowsAffected;

        try
        {
            rowsAffected = _database.Delete(beginningEvent);
        }
        catch (Exception e)
        {
            throw new DatabaseCommitFailedException("Error occurred while saving the record", e);
        }

        ValidateAffectedRows(rowsAffected);
    }

    private static void ValidateAffectedRows(int rowsAffected)
    {
        if (rowsAffected != 1)
        {
            throw new DatabaseCommitFailedException(
                $"Database reported that {rowsAffected} rows were affected. It was expected that one record would be affect.");
        }
    }

    public override BeginningEvent GetBeginningEvent(int? id = null)
    {
        if (id is null) throw new NullReferenceException("Id cannot be null.");

        return _database.Get<BeginningEvent>(id);
    }

    public override ObservableCollection<BeginningEvent> GetBeginningEvents()
    {
        return new ObservableCollection<BeginningEvent>(_database.GetAllWithChildren<BeginningEvent>());
    }

    public override ObservableCollection<T> GetObservableCollection<T>()
    {
        return new ObservableCollection<T>(_database.GetAllWithChildren<T>());
    }
}
