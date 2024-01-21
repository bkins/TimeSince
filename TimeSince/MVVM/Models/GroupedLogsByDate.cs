namespace TimeSince.MVVM.Models;

public class GroupedLogsByDate
{
    public string        Key       { get; set; }
    public List<LogLine> Logs      { get; set; }
    public string        GroupIcon { get; set; }

    public void Clear()
    {
        throw new NotImplementedException();
    }
}
