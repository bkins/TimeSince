using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TimeSince.Avails.Extensions;
using TimeSince.MVVM.Models;

namespace TimeSince.Avails;

public class Logger : ILogger
{
    private const string LogIsEmpty = "Log is empty or there are not entries that match your search criteria.";

    public static List<LogLine> LogList { get; set; }

    public bool ShouldLogToConsole   { get; set; } = true;
    public bool ShouldLogToFile      { get; set; } = true;
    public bool ShouldLogToAppCenter { get; set; } = true;

    public string FullLogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder
                                                                                           .LocalApplicationData)
                                                    , "Logger.txt");
    public StringBuilder LogStringBuilder { get; set; }
    public bool          Ascending        { get; set; }

    public string CompleteLog
    {
        get => LogStringBuilder.ToString();
        private set { }
    }
    public Logger()
    {
        LogStringBuilder = new StringBuilder(GetFileContents());
        CompleteLog      = LogStringBuilder.ToString();
        LogList          = Deserialize(CompleteLog);
    }

    public void LogError(Exception exception
                       , string    extraDetails = "")
    {
        LogError(exception.Message
               , exception.StackTrace
               , extraDetails
               , exception);
    }

    private void LogErrorToAppCenter(Exception exception
                                          , string    extraDetails = "")
    {
        Dictionary<string, string> properties = null;

        if (extraDetails.HasValue())
        {
            properties = new Dictionary<string, string>{{ nameof(extraDetails), extraDetails }};
        }

        try
        {

            App.AppServiceMethods.TrackError(exception
                                           , properties
                                           , null);
        }
        catch (Exception e)
        {
            var line = AddToLogList(e.Message
                                  , Category.Error
                                  , e.StackTrace
                                  , extraDetails);
            LogToFile();
        }
    }

    public void LogError(string    message
                       , string    exceptionDetails
                       , string    extraDetails
                       , Exception exception = null)
    {
        var line = AddToLogList(message
                              , Category.Error
                              , exceptionDetails
                              , extraDetails);

        if (ShouldLogToConsole)   Console.WriteLine(line);
        if (ShouldLogToFile)      LogToFile();
        if (ShouldLogToAppCenter) LogErrorToAppCenter(exception, extraDetails);
    }

    public void LogTrace(string message)
    {
        var line = AddToLogList(message
                              , Category.Information
                              , string.Empty
                              , string.Empty);

        Console.WriteLine(line);

        LogToFile();
    }

    private void LogToFile()
    {
        var currentlyLogged   = GetFileContents();
        var currentLoggedList = JsonConvert.DeserializeObject<List<LogLine>>(currentlyLogged) ?? new List<LogLine>();

        currentLoggedList.AddRange(LogList);

        using var streamWriter = new StreamWriter(File.Create(FullLogPath));
        streamWriter.Write(Serialize(LogList));
    }

    private LogLine AddToLogList(string   message
                               , Category category
                               , string   exceptionDetails
                               , string   extraDetails)
    {
        extraDetails = extraDetails.IsNullEmptyOrWhitespace()
                                        ? string.Empty
                                        : $"{extraDetails}";

        var line = new LogLine
                   {
                       Category     = category
                     , Message      = message
                     , ExtraDetails = $"{exceptionDetails}{extraDetails}"
                   };

        LogList.Add(line);

        return line;
    }

    private string Serialize(List<LogLine> list) => JsonConvert.SerializeObject(list);

    private string GetFileContents()
    {
        var fileContents = File.Exists(FullLogPath)
                                  ? File.ReadAllText(FullLogPath)
                                  : string.Empty;

        return fileContents;
    }
    public IOrderedEnumerable<LogLine> ToggleLogListOrderByTimeStamp(SearchOptions options)
    {
        return Ascending
            ? ToListOrderedByTimeStampDescending(options)
            : ToListOrderedByTimeStampAscending(options);
    }

    private IOrderedEnumerable<LogLine> ToListOrderedByTimeStampAscending(SearchOptions options)
    {
        Ascending = !Ascending;

        if (options.SearchTerm.IsNullEmptyOrWhitespace())
        {
            return ToList().Where(fields => FilterOptionsByCategory(options, fields))
                           .OrderBy(fields => fields.TimestampDateTime);
        }

        return ToList().Where(fields => FilterBySearchTerm(options, fields)
                                     && FilterOptionsByCategory(options, fields))
                       .OrderBy(fields => fields.TimestampDateTime);
    }

    private IOrderedEnumerable<LogLine> ToListOrderedByTimeStampDescending(SearchOptions options)
    {
        Ascending = !Ascending;

        if (options.SearchTerm.IsNullEmptyOrWhitespace())
        {
            return ToList().Where(fields => FilterOptionsByCategory(options, fields))
                           .OrderByDescending(fields => fields.TimestampDateTime);
        }

        return ToList().Where(fields => FilterBySearchTerm(options, fields)
                                     && FilterOptionsByCategory(options, fields))
                       .OrderByDescending(fields => fields.TimestampDateTime);
    }

    public ObservableCollection<LogLine> SearchLogAsList(SearchOptions options)
    {
        return new ObservableCollection<LogLine>(LogList.Where(fields => FilterBySearchTerm(options
                                                                                          , fields)
                                                                      && FilterOptionsByCategory(options
                                                                                               , fields)));
    }

    private bool FilterBySearchTerm(SearchOptions options
                                         , LogLine       fields)
    {
        return (fields.TimeStamp.Contains(options.SearchTerm
                                        , StringComparison.OrdinalIgnoreCase)
             || fields.Category.ToString().Contains(options.SearchTerm
                                                  , StringComparison.OrdinalIgnoreCase)
             || fields.Message.Contains(options.SearchTerm
                                      , StringComparison.OrdinalIgnoreCase));
    }

    private bool FilterOptionsByCategory(SearchOptions options
                                              , LogLine       fields)
    {
        return (fields.Category == Category.Error && options.ShowErrors)
            || (fields.Category == Category.Warning && options.ShowWarnings)
            || (fields.Category == Category.Information && options.ShowInformation);
    }

    public string SearchLog(SearchOptions options)
    {
        var resultsList = SearchLogAsList(options).ToList();

        return ListToString(resultsList);
    }

    private string ListToString(List<LogLine> list)
    {
        var log = new StringBuilder();

        foreach (var line in list)
        {
            log.AppendLine(line.ToString(true));
        }

        return log.Length == 0
            ? LogIsEmpty
            : log.ToString();
    }
    public List<LogLine> ToList(bool forceRefresh = false)
    {
        if (!forceRefresh) { return LogList; }

        var task = Task.Factory.StartNew(() => Deserialize(GetFileContents()));

        Task.WaitAll();

        return task.Result;

        //return Deserialize(GetFileContents());
    }
    private string ToStringOrderedByTimeStampDescending(SearchOptions options)
    {
        Ascending = !Ascending;

        var theList = ToListOrderedByTimeStampDescending(options);

        return ListToString(theList.ToList());
    }

    private string ToStringOrderedByTimeStampAscending(SearchOptions options)
    {
        Ascending = !Ascending;

        var theList = ToListOrderedByTimeStampAscending(options);

        return ListToString(theList.ToList());
    }

    public List<LogLine> Deserialize(string json)
    {
        return json.IsValidJson()
            ? JsonConvert.DeserializeObject<List<LogLine>>(json)
            : LegacyLogFileToList(json);
    }

    private List<LogLine> LegacyLogFileToList(string fileContents)
    {
        var fileLines = new List<string>(fileContents.Split(new[] { Environment.NewLine }
                                                          , StringSplitOptions.RemoveEmptyEntries));
        var logLines = (
            from line in fileLines
            select line.Split(']')
            into lineArray
            let lineTimeStamp = lineArray[0]
                                .Replace("["
                                       , "")
                                .Trim()
            let categoryMessage = lineArray.Length > 1 ? lineArray[1].Split(':') : lineArray[0].Split(':')
            let lineCategory = categoryMessage[0].Trim()
            let lineMessage = categoryMessage[1].Trim()
            select new LogLine
                   {
                       TimeStamp = lineTimeStamp
                     , Category  = GetEnum(lineCategory)
                     , Message   = lineMessage
                   }).ToList();

        return logLines;

        // return fileLines.Select(line => new LogLine { Message = line })
        //                 .ToList();
    }
    
    public Category GetEnum(string enumName)
    {
        return enumName switch
               {
                   nameof(Category.Error) => Category.Error
                 , nameof(Category.Warning) => Category.Warning
                 , nameof(Category.Information) => Category.Information
                 , _ => Category.Unknown
               };
    }
    
    public void Log<TState>(LogLevel                        logLevel
                          , EventId                         eventId
                          , TState                          state
                          , Exception                       exception
                          , Func<TState, Exception, string> formatter)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
    public void Clear()
    {
        LogList.Clear();
        LogStringBuilder.Clear();
        CompleteLog = string.Empty;

        File.Delete(FullLogPath);
        File.Create(FullLogPath);
    }
}

public enum Category
{
    Error       = 0
  , Warning     = 1
  , Information = 2
  , Unknown     = 4
}

[Flags]
public enum LogLevels
{
    Errors      = 0b_0000_0000 // 0
  , Warnings    = 0b_0000_0001 // 1
  , Information = 0b_0000_0010 // 2
  , NoLogging   = 0b_0000_0100 // 4
  , All         = Errors | Warnings | Information
}
