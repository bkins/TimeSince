using SQLite;

namespace TimeSince.MVVM.BaseClasses;

public class BaseModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
}
