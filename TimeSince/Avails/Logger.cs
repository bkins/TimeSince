using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.Models;

namespace TimeSince.Avails;

public partial class Logger : ILogger
{
    public bool ShouldLogToFile      { get; set; } = true;
    public bool ShouldLogToConsole   { get; set; } = true;
    public bool ShouldLogToAppCenter { get; set; } = true;
    public bool ShouldLogToToast     { get; set; } = false;

    public string ExtraDetails { get; set; }

    public string FullLogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder
                                                                                           .LocalApplicationData)
                                                    , "Logger.txt");
    public static List<LogLine> LogList { get; set; }

    public StringBuilder LogStringBuilder { get; private set; }

    public Logger()
    {
        var extraDetailsBuilder = new StringBuilder();
        extraDetailsBuilder.AppendLine("");
        extraDetailsBuilder.AppendLine($"UserId: {PreferencesDataStore.ErrorReportingId}");
        extraDetailsBuilder.AppendLine("Info From Device:");
        extraDetailsBuilder.AppendLine(App.AppServiceMethods.FullDeviceInfo());
        extraDetailsBuilder.AppendLine("");
        extraDetailsBuilder.AppendLine($"App Info:");
        extraDetailsBuilder.AppendLine($"\tVersion: {App.AppServiceMethods.AppInfo.CurrentVersion}");
        extraDetailsBuilder.AppendLine($"\tBuild:   {App.AppServiceMethods.AppInfo.CurrentBuild}");

        ExtraDetails = extraDetailsBuilder.ToString();

        RefreshLogsFromFile();
    }

    public void RefreshLogsFromFile()
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

    public void LogError(string    message
                             , string    exceptionDetails
                             , string    extraDetails
                             , Exception exception = null)
    {
        ExtraDetails += extraDetails;

        var line = AddToLogList(message
                              , Category.Error
                              , exceptionDetails
                              , extraDetails);

        if (ShouldLogToConsole)   LogToConsole(line);
        if (ShouldLogToFile)      LogToFile();
        if (ShouldLogToAppCenter) LogErrorToAppCenter(exception, extraDetails);
        if (ShouldLogToToast)     ToastMessage(line.Message);
        //if (sendEmail)            await SendEmail().ConfigureAwait(false);
    }

    public void LogEvent(string                     name
                       , IDictionary<string,string> properties)
    {
        var extraDetails = new StringBuilder();

        if (properties is not null)
        {
            foreach (var property in properties)
            {
                extraDetails.AppendLine($"{property.Key}:");
                extraDetails.AppendLine(property.Value);
            }
        }

        ExtraDetails += extraDetails.ToString();

        var line = AddToLogList(name
                              , Category.Event
                              , string.Empty
                              , ExtraDetails);

        if (ShouldLogToConsole)   LogToConsole(line);
        if (ShouldLogToFile)      LogToFile();
        if (ShouldLogToToast)     ToastMessage(line.Message);
    }

    public void LogTrace(string message)
    {
        var line = AddToLogList(message
                              , Category.Information
                              , string.Empty
                              , string.Empty);

        LogToConsole(line);
        LogToFile();

        if (ShouldLogToToast) ToastMessage(line.Message);
    }

    public void ToastMessage(string message)
    {
        ToastLineMessage(message);
    }

    public IOrderedEnumerable<LogLine> ToggleLogListOrderByTimeStamp(SearchOptions options)
    {
        return Ascending
            ? ToListOrderedByTimeStampDescending(options)
            : ToListOrderedByTimeStampAscending(options);
    }

    public string SearchLog(SearchOptions options)
    {
        var resultsList = SearchLogAsList(options).ToList();

        return ListToString(resultsList);
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

    public void DeleteLogEntry(LogLine logEntry)
    {
        var comparer = new LogLine.LogLineComparer();
        if ( ! LogList.Any(entry => comparer.Equals(entry, logEntry))) return;

        var entryToRemove = LogList.First(entry => comparer.Equals(entry, logEntry));

        LogList.Remove(entryToRemove);
        SaveLogToFile();
    }

    public List<LogLine> Deserialize(string json)
    {
        return json.IsValidJson()
                        ? JsonConvert.DeserializeObject<List<LogLine>>(json)
                        : LegacyLogFileToList(json);
    }

    //BENDO: Move to service
    private async Task SendEmail()
    {
        try
        {
            var emailBody = BuildEmailBody();

            var message = new EmailMessage("TimeSince log entry: "
                                         , emailBody
                                         , "BenHop@gmail.com");

            await Email.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException notSupportedException)
        {
            App.Logger.LogError(notSupportedException);
        }
        catch (Exception ex)
        {
            App.Logger.LogError(ex);
        }
    }

    private static string BuildEmailBody()
    {
        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine(PreferencesDataStore.ErrorReportingId);
        bodyBuilder.AppendLine(Serialize(LogList));

        var emailBody = bodyBuilder.ToString();

        return emailBody;
    }

}

public enum Category
{
    Error       = 0
  , Warning     = 1
  , Information = 2
  , Event       = 3
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
