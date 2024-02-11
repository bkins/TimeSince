using System.Text;
using Newtonsoft.Json;
using TimeSince.Avails.Extensions;
using TimeSince.MVVM.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace TimeSince.Avails;

public partial class Logger
{
    private string CompleteLog
    {
        get => LogStringBuilder.ToString();
        set { }
    }

    private void LogErrorToAppCenter(Exception? exception
                                   , string    extraDetails = "")
    {
        Dictionary<string, string>? properties = null;

        if (extraDetails.HasValue())
        {
            properties = new Dictionary<string, string>{{ nameof(extraDetails), extraDetails }};
        }

        try
        {
            App.AppServiceMethods.TrackError(exception
                                           , properties ?? null
                                           , null);
        }
        catch (Exception e)
        {
            var resultForDebugging = AddToLogList(e.Message
                                                , Category.Error
                                                , e.StackTrace ?? string.Empty
                                                , extraDetails);
            LogToFile();
        }
    }

    private static void LogToConsole(LogLine line)
    {
        Console.WriteLine(line);
    }

    private void LogToFile()
    {
        var currentlyLogged   = GetFileContents();
        var currentLoggedList = JsonConvert.DeserializeObject<List<LogLine>>(currentlyLogged) ?? new List<LogLine>();

        if (LogList == null) return;

        currentLoggedList.AddRange(LogList);

        using var streamWriter = new StreamWriter(File.Create(FullLogPath));
        streamWriter.Write(Serialize(LogList));
    }

    private void SoftClearLogFile()
    {
        using var streamWriter = new StreamWriter(File.Create(FullLogPath));
        streamWriter.Write(string.Empty);
    }



    private static LogLine AddToLogList(string   message
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

        LogList?.Add(line);

        return line;
    }

    private static string Serialize(List<LogLine>? list) => JsonConvert.SerializeObject(list);

    private string GetFileContents()
    {
        return File.Exists(FullLogPath)
                        ? File.ReadAllText(FullLogPath)
                        : string.Empty;
    }

    private List<LogLine>? LegacyLogFileToList(string fileContents)
    {
        var fileLines = new List<string>(fileContents.Split([Environment.NewLine]
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

    private static Category GetEnum(string enumName)
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

        // using var stream = File.OpenWrite(FullLogPath);
        // var       data   = Encoding.UTF8.GetBytes(serializedLog);
        // stream.Write(data, 0, data.Length);

        File.WriteAllText(FullLogPath, serializedLog);
    }
    private static void ToastLineMessage(string?        text
                                       , ToastDuration duration = ToastDuration.Short
                                       , double        fontSize = 14)
    {
        var toast = Toast.Make(text
                             , duration
                             , fontSize);
        toast.Show();
    }
}
