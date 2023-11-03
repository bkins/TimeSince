namespace TimeSince.Data;

public static class Settings
{
    public static string PrimaryColorAsHex
    {
        get => Preferences.Get(nameof(PrimaryColorAsHex), string.Empty);
        set => Preferences.Set(nameof(PrimaryColorAsHex), value);
    }

    public static string SecondaryColorAsHex
    {
        get => Preferences.Get(nameof(SecondaryColorAsHex), string.Empty);
        set => Preferences.Set(nameof(SecondaryColorAsHex), value);
    }


    public static string TertiaryColorAsHex
    {
        get => Preferences.Get(nameof(TertiaryColorAsHex), string.Empty);
        set => Preferences.Set(nameof(TertiaryColorAsHex), value);
    }
}
