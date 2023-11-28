
using System.Dynamic;
using Microsoft.AppCenter.Crashes;

namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService //AppCenter Service Methods
{
    /// <summary>
    /// Check whether the SDK is enabled or not as a whole.
    /// </summary>
    /// <returns>A task with result being true if enabled, false if disabled.</returns>
    public async Task<bool> GetIsAppCenterEnabledAsync()
    {
        return await _appCenterService.GetIsAppCenterEnabledAsync();
    }

    /// <summary>
    ///Enable or disable the SDK as a whole.
    /// Updating the state propagates the value to all services that have been started.
    /// </summary>
    /// <param name="enabled">True to enable; False to disable.</param>
    public async Task SetIsAppCenterEnabledAsync(bool enabled)
    {
        await _appCenterService.SetIsAppCenterEnabledAsync(enabled);
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
    public void TrackEvent(string name, IDictionary<string, string> properties = null)
    {
        _appCenterService.TrackEvent(name, properties);
    }

    public void TrackError(Exception            exception,
                           IDictionary<string, string> properties = null,
                           params ErrorAttachmentLog[] attachments)
    {
        Crashes.TrackError(exception, properties, attachments);
    }
}
