using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TimeSince.Avails.Attributes;
using TimeSince.Avails.SecretsEnums;
using TimeSince.Services.ServicesIntegration;

namespace TimeSince.Services;

public class AppCenterService
{
    private const string LogTag = "com.hopkins.timeSince.AppCenterService";

    public void Start()
    {
        AppCenter.LogLevel = LogLevel.Verbose;

        DefineEvents();

        Crashes.ShouldProcessErrorReport    = ShouldProcess;
        Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
        Crashes.GetErrorAttachments         = GetErrorAttachments;


        var appCenterSecretKey = AppIntegrationService.GetSecretValue(SecretCollections.AppCenter
                                                                    , SecretKeys.AppSecretKey);
        AppCenter.Start($"android={appCenterSecretKey};"
                      , typeof(Analytics)
                      , typeof(Crashes));

        AppCenter.GetInstallIdAsync().ContinueWith(installId =>
        {
            AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
        });

        Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
        {
            AppCenterLog.Info(LogTag, $"Crashes.HasCrashedInLastSession={hasCrashed.Result}");
        });

        Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
        {
            const string eventName = "Crashes.LastSessionCrashReport";
            AppCenterLog.Info(LogTag, $"{eventName}.StackTrace= {report.Result?.StackTrace}");

            TrackEvent(eventName
                     , new Dictionary<string, string>
                       { {
                             $"{eventName}.StackTrace"
                           , report.Result?.StackTrace ?? string.Empty
                         } });

        });
    }

    private static void DefineEvents()
    {
        Crashes.SendingErrorReport      += SendingErrorReportHandler;
        Crashes.SentErrorReport         += SentErrorReportHandler;
        Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
    }

    private static bool ShouldProcess(ErrorReport report)
    {
        AppCenterLog.Info(LogTag, "Determining whether to process error report");
        return true;
    }

    private static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
    {
        AppCenterLog.Info(LogTag, "Sending error report");

        var report = e.Report;

        if (report.StackTrace != null)
        {
            AppCenterLog.Info(LogTag, report.StackTrace);
        }
        else if (report.AndroidDetails != null)
        {
            AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
        }
    }

    private static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
    {
        AppCenterLog.Info(LogTag, "Sent error report");

        var report = e.Report;

        AppCenterLog.Info(LogTag
                        , report.StackTrace ?? "No system StackTrace was found");

        if (report.AndroidDetails != null)
        {
            AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
        }
    }

    private static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
    {
        AppCenterLog.Info(LogTag, "Failed to send error report");

        var report = e.Report;

        if (report.StackTrace != null)
        {
            AppCenterLog.Info(LogTag, report.StackTrace);
        }
        else if (report.AndroidDetails != null)
        {
            AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
        }

        if (e.Exception != null)
        {
            AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
        }
    }

    public static bool ConfirmationHandler()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current
                       ?.MainPage
                       ?.DisplayActionSheet("Crash detected. Send anonymous crash report?"
                                          , null
                                          , null
                                          , "Send"
                                          , "Always Send"
                                          , "Don't Send")
                       .ContinueWith(arg =>
                       {
                           var answer = arg.Result;

                           var userConfirmationSelection = answer switch
                                                           {
                                                               "Send" => UserConfirmation.Send
                                                             , "Always Send" => UserConfirmation.AlwaysSend
                                                             , _ => UserConfirmation.DontSend
                                                           };

                           AppCenterLog.Debug(LogTag
                                            , $"User selected confirmation option: \"{answer}\"");
                           Crashes.NotifyUserConfirmation(userConfirmationSelection);
                       });
        });

        return true;
    }

    /// <summary>
    /// Check whether the SDK is enabled or not as a whole.
    /// </summary>
    /// <returns>A task with result being true if enabled, false if disabled.</returns>
    public async Task<bool> GetIsAppCenterEnabledAsync()
    {
        return await AppCenter.IsEnabledAsync();
    }

    /// <summary>
    ///Enable or disable the SDK as a whole.
    /// Updating the state propagates the value to all services that have been started.
    /// </summary>
    /// <param name="enabled">True to enable; False to disable.</param>
    public static async Task SetIsAppCenterEnabledAsync(bool enabled)
    {
        await AppCenter.SetEnabledAsync(enabled);
    }

    /// <summary>
    /// Track a custom event with name and optional properties.
    /// </summary>
    /// <param name="name">An event name.</param>
    /// <param name="properties">Optional properties.</param>
    /// <remarks>The name parameter can not be null or empty.
    /// Maximum allowed length = 256.
    /// The properties parameter maximum item count = 20.
    /// The properties keys/names can not be null or empty, maximum allowed key length = 125.
    /// The properties values can not be null, maximum allowed value length = 125.</remarks>
    public static void TrackEvent(string name, IDictionary<string, string> properties)
    {
        Analytics.TrackEvent(name, properties);
        App.Logger.LogEvent(name, properties);
    }

    [UnderConstruction("Might not need, but want to explore this later.")]
    private static IEnumerable<ErrorAttachmentLog> GetErrorAttachments(ErrorReport report)
    {
        return new ErrorAttachmentLog[]
               {
                   ErrorAttachmentLog.AttachmentWithText("Hello world!"
                                                       , "hello.txt")
                 , ErrorAttachmentLog.AttachmentWithBinary("Fake image"u8.ToArray()
                                                         , "fake_image.jpeg"
                                                         , "image/jpeg")
               };
    }
}
