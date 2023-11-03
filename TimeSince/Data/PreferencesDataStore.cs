using System.Collections.ObjectModel;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Storage;
using TimeSince.MVVM.Models;

namespace TimeSince.Data;

public static class PreferencesDataStore
{
    public static string PrimaryColorName
    {
        get => Preferences.Get(nameof(PrimaryColorName), null);
        set => Preferences.Set(nameof(PrimaryColorName), value);
    }

    public static string SecondaryColorName
    {
        get => Preferences.Get(nameof(SecondaryColorName), null);
        set => Preferences.Set(nameof(SecondaryColorName), value);
    }

    public static string TertiaryColorName
    {
        get => Preferences.Get(nameof(TertiaryColorName), null);
        set => Preferences.Set(nameof(TertiaryColorName), value);
    }

    public static string DefaultPrimaryColorName
    {
        get => Preferences.Get(nameof(DefaultPrimaryColorName), null);
        set => Preferences.Set(nameof(DefaultPrimaryColorName), value);
    }

    public static string DefaultSecondaryColorName
    {
        get => Preferences.Get(nameof(DefaultSecondaryColorName), null);
        set => Preferences.Set(nameof(DefaultSecondaryColorName), value);
    }

    public static string DefaultTertiaryColorName
    {
        get => Preferences.Get(nameof(DefaultTertiaryColorName), null);
        set => Preferences.Set(nameof(DefaultTertiaryColorName), value);
    }

    public static bool PaidToTurnOffAds
    {
        get => Preferences.Get(nameof(PaidToTurnOffAds), false);
        set => Preferences.Set(nameof(PaidToTurnOffAds), value);
    }

    public static double AwesomePersonScore
    {
        get => Preferences.Get(nameof(AwesomePersonScore), 0.0);
        set => Preferences.Set(nameof(AwesomePersonScore), value);
    }

    public static void ClearColors()
    {
        //Preferences.Clear();
        PrimaryColorName   = null;
        SecondaryColorName = null;
        TertiaryColorName  = null;
    }
}
