﻿using TimeSince.Services.ServicesIntegration;

namespace TimeSince.Data;

public static class PreferencesDataStore
{
    public static string PrimaryColorName
    {
        get => Preferences.Get(nameof(PrimaryColorName), "Unknown Color");
        set => Preferences.Set(nameof(PrimaryColorName), value);
    }

    public static string SecondaryColorName
    {
        get => Preferences.Get(nameof(SecondaryColorName), "Unknown Color");
        set => Preferences.Set(nameof(SecondaryColorName), value);
    }

    public static string TertiaryColorName
    {
        get => Preferences.Get(nameof(TertiaryColorName), "Unknown Color");
        set => Preferences.Set(nameof(TertiaryColorName), value);
    }

    public static string DefaultPrimaryColorName
    {
        get => Preferences.Get(nameof(DefaultPrimaryColorName), "Unknown Color");
        set => Preferences.Set(nameof(DefaultPrimaryColorName), value);
    }

    public static string DefaultSecondaryColorName
    {
        get => Preferences.Get(nameof(DefaultSecondaryColorName), "Unknown Color");
        set => Preferences.Set(nameof(DefaultSecondaryColorName), value);
    }

    public static string DefaultTertiaryColorName
    {
        get => Preferences.Get(nameof(DefaultTertiaryColorName), "Unknown Color");
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

    public static int LastSortOption
    {
        get => Preferences.Get(nameof(LastSortOption), 0);
        set => Preferences.Set(nameof(LastSortOption), value);
    }

    public static bool HideStartupMessage
    {
        get => Preferences.Get(nameof(HideStartupMessage), false);
        set => Preferences.Set(nameof(HideStartupMessage), value);
    }

    public static string ErrorReportingId
    {
        get
        {
            if (AppIntegrationService.IsTesting) return "Testing - No Id available";

            var id = Preferences.Get(nameof(ErrorReportingId), null);

            if (id != null) return id;

            id = Guid.NewGuid().ToString();
            Preferences.Set(nameof(ErrorReportingId), id);

            return id;
        }
        set => Preferences.Set(nameof(ErrorReportingId), value);
    }

    public static void ClearColors()
    {
        PrimaryColorName   = null;
        SecondaryColorName = null;
        TertiaryColorName  = null;
    }
}
