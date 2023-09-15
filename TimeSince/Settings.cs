namespace TimeSince;

public static class Settings
{
    public static DateTime BeginDateTime
    {
        get => Preferences.Default.Get(nameof(BeginDateTime)
                                     , DateTime.Now);
        set => Preferences.Default.Set(nameof(BeginDateTime)
                                     , value);
    }

    public static string BeginDateTimeName
    {
        get => Preferences.Default.Get(nameof(BeginDateTimeName)
                                     , string.Empty);
        set => Preferences.Default.Set(nameof(BeginDateTimeName)
                                     , value);
    }
}
