using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using TimeSince.Avails.Extensions;
using TimeSince.MVVM.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core; //BENDO: Need to update to 7.0.1 (at least), but need to upgrade to .NET 8 first.

namespace TimeSince.Avails;

public partial class Logger
{
    private const string LogIsEmpty = "Log is empty or there are not entries that match your search criteria.";

    private bool          Ascending        { get; set; }

    private string CompleteLog
    {
        get => LogStringBuilder.ToString();
        set { }
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

    private static void LogToConsole(LogLine line)
    {

        Console.WriteLine(line);
    }

    private IEnumerable<LogLine> SearchLogAsList(SearchOptions options)
    {
        return new ObservableCollection<LogLine>(LogList.Where(fields => FilterBySearchTerm(options
                                                                                          , fields)
                                                                      && FilterOptionsByCategory(options
                                                                                               , fields)));
    }

    private List<LogLine> ToList(bool forceRefresh = false)
    {
        if (!forceRefresh) { return LogList; }

        var task = Task.Factory.StartNew(() => Deserialize(GetFileContents()));

        Task.WaitAll();

        return task.Result;

        //return Deserialize(GetFileContents());
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

    private static string Serialize(List<LogLine> list) => JsonConvert.SerializeObject(list);

    private string GetFileContents()
    {
        var fileContents = File.Exists(FullLogPath)
                                  ? File.ReadAllText(FullLogPath)
                                  : string.Empty;

        return fileContents;
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
    }

    private Category GetEnum(string enumName)
    {
        return enumName switch
               {
                   nameof(Category.Error) => Category.Error
                 , nameof(Category.Warning) => Category.Warning
                 , nameof(Category.Information) => Category.Information
                 , nameof(Category.Event) => Category.Event
                 , _ => Category.Unknown
               };
    }

    private void SaveLogToFile()
    {
        var serializedLog = Serialize(LogList);
        File.WriteAllText(FullLogPath, serializedLog);
    }

    private static void ToastLineMessage(string            text
                                       , ToastDuration     duration = ToastDuration.Short
                                       , double            fontSize = 14)
    {
        var toast = Toast.Make(text
                             , duration
                             , fontSize);

        toast.Show();
    }
}
