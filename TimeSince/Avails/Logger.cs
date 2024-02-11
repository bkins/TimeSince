using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.Models;
using TimeSince.Services.ServicesIntegration;

namespace TimeSince.Avails;

public partial class Logger : ILogger
{
    public bool ShouldLogToFile      { get; set; } = true;
    public bool ShouldLogToConsole   { get; set; } = true;
    public bool ShouldLogToAppCenter { get; set; } = true;
    public bool ShouldLogToToast     { get; set; } = false;
    public bool ShouldLogToEmail     { get; set; } = false;

    public string ExtraDetails { get; set; }

    public string FullLogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder
                                                                                           .LocalApplicationData)
                                                    , "Logger.txt");
    public static List<LogLine>? LogList { get; set; }

    public StringBuilder LogStringBuilder { get; private set; }

    public Logger(bool isTesting = false)
    {
        AppIntegrationService.IsTesting = isTesting;

        var extraDetailsBuilder = new StringBuilder();

        extraDetailsBuilder.AppendLine("");
        extraDetailsBuilder.AppendLine($"UserId: {PreferencesDataStore.ErrorReportingId}");
        extraDetailsBuilder.AppendLine("Info From Device:");
        extraDetailsBuilder.AppendLine(AppIntegrationService.FullDeviceInfo());
        extraDetailsBuilder.AppendLine("");
        extraDetailsBuilder.AppendLine("App Info:");
        extraDetailsBuilder.AppendLine($"\tVersion: {AppIntegrationService.AppInfo?.CurrentVersion}");
        extraDetailsBuilder.AppendLine($"\tBuild:   {AppIntegrationService.AppInfo?.CurrentBuild}");

        ExtraDetails = extraDetailsBuilder.ToString();

        LogStringBuilder = new StringBuilder(GetFileContents());
        CompleteLog      = LogStringBuilder.ToString();
        LogList          = Deserialize(CompleteLog);
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
               , exception.StackTrace ?? string.Empty
               , extraDetails
               , exception);
    }

    public void LogError(string     message
                       , string     exceptionDetails
                       , string     extraDetails
                       , Exception? exception = null)
    {
        ExtraDetails += extraDetails;

        var line = AddToLogList(message
                              , Category.Error
                              , exceptionDetails
                              , extraDetails);

        if (ShouldLogToConsole)                                        LogToConsole(line);
        if (ShouldLogToFile)                                           LogToFile();
        if (ShouldLogToAppCenter && ! AppIntegrationService.IsTesting) LogErrorToAppCenter(exception, extraDetails);
        if (ShouldLogToToast && ! AppIntegrationService.IsTesting)     ToastMessage(line.Message);
        if (ShouldLogToEmail && ! AppIntegrationService.IsTesting)     SendEmail();
    }

    public void LogEvent(string                      name
                       , IDictionary<string, string> properties)
    {
        var extraDetails = new StringBuilder();

        foreach (var property in properties)
        {
            extraDetails.AppendLine($"{property.Key}:");
            extraDetails.AppendLine(property.Value);
        }

        ExtraDetails += extraDetails.ToString();

        var line = AddToLogList(name
                              , Category.Event
                              , string.Empty
                              , ExtraDetails);

        if (ShouldLogToConsole)                                    LogToConsole(line);
        if (ShouldLogToFile)                                       LogToFile();
        if (ShouldLogToToast && ! AppIntegrationService.IsTesting) ToastMessage(line.Message);
        if (ShouldLogToEmail && ! AppIntegrationService.IsTesting) SendEmail();
    }

    public void LogTrace(string message)
    {
        var line = AddToLogList(message
                              , Category.Information
                              , string.Empty
                              , string.Empty);

        LogToConsole(line);
        LogToFile();

        if (ShouldLogToToast && ! AppIntegrationService.IsTesting) ToastMessage(line.Message);
    }

    public static void ToastMessage(string? message)
    {
        ToastLineMessage(message);
    }

    public void Clear(bool softClearLogFile = false)
    {
        LogList?.Clear();
        LogStringBuilder.Clear();
        CompleteLog = string.Empty;

        if (softClearLogFile)
        {
            SoftClearLogFile();
        }
        else
        {
            File.Delete(FullLogPath);
            File.Create(FullLogPath);
        }

    }

    public void DeleteLogEntry(LogLine logEntry)
    {
        var comparer = new LogLine.LogLineComparer();

        if ( LogList != null
          && ! LogList.Any(entry => comparer.Equals(entry, logEntry))) return;

        if (LogList != null)
        {
            var entryToRemove = LogList.First(entry => comparer.Equals(entry, logEntry));

            LogList.Remove(entryToRemove);
        }

        SaveLogToFile();
    }

    public List<LogLine>? Deserialize(string json)
    {
        return json.IsValidJson()
                        ? JsonConvert.DeserializeObject<List<LogLine>>(json)
                        : LegacyLogFileToList(json);
    }

    private static void SendEmail()
    {
        App.AppServiceMethods.ComposeEmail(BuildEmailBody()
                                         , "BenHopApps@gmail.com"
                                         , "TimeSince log entry: ");
    }

    private static string BuildEmailBody()
    {
        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine(PreferencesDataStore.ErrorReportingId);
        bodyBuilder.AppendLine(Serialize(LogList));

        var emailBody = bodyBuilder.ToString();

        return emailBody;
    }

    public void Log<TState>(LogLevel                        logLevel
                          , EventId                         eventId
                          , TState                          state
                          , Exception?                      exception
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
