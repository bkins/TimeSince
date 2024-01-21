using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.Models;
using MauiGraphics = Microsoft.Maui.Graphics;
using Color = System.Drawing.Color;

namespace TimeSince.Avails.ColorHelpers;

public static class ColorUtility
{
    public static ObservableCollection<ColorName>? ColorNames { get; set; }

    public static List<string?> GetNamedColors()
    {
        List<string?> namedColors = typeof(Color).GetProperties(BindingFlags.Public
                                                              | BindingFlags.Static)
                                                 .Where(propertyInfo => propertyInfo.PropertyType == typeof(Color))
                                                 .Select(propertyInfo => propertyInfo.Name)
                                                 .ToList();

        return namedColors;
    }

    public static MauiGraphics.Color? ConvertSystemColorNameToMauiColor(string? systemColorName
                                                                      , Color? defaultColor = null)
    {
        var systemColor = GetColorFromName(systemColorName
                                         , defaultColor);

        return ConvertSystemColorToMauiColor(systemColor);
    }

    private static Color? GetColorFromName(string? colorName
                                         , Color?  defaultColor = null)
    {
        if (colorName is null) return defaultColor;

        var colorProperty = typeof(Color).GetProperty(colorName
                                                    , BindingFlags.Public
                                                    | BindingFlags.Static);

        if (colorProperty != null
         && colorProperty.PropertyType == typeof(Color))
        {
            return (Color)colorProperty.GetValue(null)!;
        }

        return defaultColor;
    }

    private static MauiGraphics.Color? ConvertSystemColorToMauiColor(Color? systemColor)
    {
        if (systemColor is null) return null;

        return new MauiGraphics.Color(systemColor?.R ?? 0
                                    , systemColor?.G ?? 0
                                    , systemColor?.B ?? 0
                                    , systemColor?.A ?? 0);
    }

    public static Color ConvertMauiColorToSystemColor(MauiGraphics.Color? mauiColor)
    {
        if (mauiColor is null) return Color.Empty;

        var alpha = (int)(mauiColor.Alpha * 255);
        var red   = (int)(mauiColor.Red * 255);
        var green = (int)(mauiColor.Green * 255);
        var blue  = (int)(mauiColor.Blue * 255);

        return Color.FromArgb(alpha
                            , red
                            , green
                            , blue);
    }


    public static void SetResourceColors()
    {
        UpdateColorResource(ResourceColors.Primary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.PrimaryColorName
                                                                         , Color.RoyalBlue)));
        UpdateColorResource(ResourceColors.Secondary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.SecondaryColorName
                                                                         , Color.Lavender)));
        UpdateColorResource(ResourceColors.Tertiary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.TertiaryColorName
                                                                         , Color.Indigo)));
    }

    public static void SetDefaultResourceColors()
    {
        UpdateColorResource(ResourceColors.Primary
                          , MauiGraphics.Color.FromArgb(ColorInfo.RoyalAzureAsHex));
        UpdateColorResource(ResourceColors.Secondary
                          , MauiGraphics.Color.FromArgb(ColorInfo.LavenderMistAsHex));
        UpdateColorResource(ResourceColors.Tertiary
                          , MauiGraphics.Color.FromArgb(ColorInfo.MidnightIndigoAsHex));

    }

    private static void SetResourceColor(string resourceColorName
                                       , string colorName
                                       , Color? defaultColor = null)
    {
        if (colorName.IsNullEmptyOrWhitespace()) return;

        if (! Application.Current
                         ?.Resources
                         .TryGetValue(resourceColorName
                                    , out _)
         ?? true)
        {
            return;
        }

        Application.Current
                   .Resources[resourceColorName] = GetColorFromName(colorName
                                                                  , defaultColor);
    }

    public static bool UpdateColorResource(ResourceColors      resourceColorName
                                         , MauiGraphics.Color? mauiColor)
    {
        if (mauiColor is null) return false;

        if (Application.Current == null) return false;

        var appResources = Application.Current.Resources;
        var colorName    = resourceColorName.ToString();

        if (! appResources.TryGetValue(colorName
                                     , out _)) return false;

        appResources[colorName] = mauiColor;

        try
        {
            UpdateColorName(resourceColorName
                          , GetNameFromColor(mauiColor
                                           , (ColorNames ?? []).ToList()));
        }
        catch (Exception e)
        {
            App.Logger.LogError(e);

            return false;
        }

        return true;
    }

    private static void UpdateColorName(ResourceColors resourceColorName
                                      , string?        name)
    {
        switch (resourceColorName)
        {
            case ResourceColors.Primary:
                PreferencesDataStore.PrimaryColorName = name;

                break;

            case ResourceColors.Secondary:
                PreferencesDataStore.SecondaryColorName = name;

                break;

            case ResourceColors.Tertiary:
                PreferencesDataStore.TertiaryColorName = name;

                break;

            case ResourceColors.InvalidColor:
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceColorName)
                                                    , resourceColorName
                                                    , null);
        }
    }

    public static MauiGraphics.Color? GetColorFromResources(ResourceColors resourceColorsName)
    {
        try
        {
            if (Application.Current
                           ?.Resources
                           .TryGetValue(resourceColorsName.ToString()
                                      , out _)
             ?? false)
            {
                return Application.Current
                                  .Resources[resourceColorsName.ToString()] as MauiGraphics.Color;
            }
        }
        catch (InvalidCastException e)
        {
            App.Logger.LogError("Error while getting color from Resources.  Converting System color to Maui color."
                              , e.Message
                              , e.StackTrace ?? string.Empty);

            return ConvertSystemColorToMauiColor((Color)Application.Current
                                                                   ?.Resources[resourceColorsName.ToString()]!);
        }
        catch (Exception e)
        {
            App.Logger.LogError("Error while getting color from Resources.  Converting System color to Maui color."
                              , e.Message
                              , e.StackTrace ?? string.Empty);

            return null;
        }

        return null;
    }

    public static int GetIndexFromPartialName(string? partialName)
    {
        if (partialName is null) return -1;

        var index = (ColorNames ?? []).ToList()
                                      .FindIndex(colorName => colorName.Name
                                                                       .StartsWith(partialName.ToLower()
                                                                                 , StringComparison.CurrentCultureIgnoreCase));

        return index;
    }

    public static string? GetNameFromColor(MauiGraphics.Color? mauiColor
                                         , List<ColorName>     colorNames)
    {
        if (mauiColor is null) return string.Empty;

        var colorName = colorNames.FirstOrDefault(name => name.Color?.ToArgbHex() == mauiColor.ToArgbHex());

        if (colorName is not null) return colorName.Name;

        var closeEnough = FindTheClosestColor(colorNames
                                            , mauiColor);

        if (closeEnough is null) return string.Empty;

        return GetNameFromColor(closeEnough.Color
                              , colorNames);
    }

    public static int GetIndexFromColor(MauiGraphics.Color? mauiColor)
    {
        var colorNamesList = (ColorNames ?? []).ToList();

        var transparent = colorNamesList.FindIndex(name => name.Name == "Transparent");

        if (mauiColor is null) return transparent;

        var index = colorNamesList.FindIndex(name => name.Color?.ToArgbHex() == mauiColor.ToArgbHex());

        if (index != -1) return index;

        var closeEnough = FindTheClosestColor(colorNamesList
                                            , mauiColor);

        return closeEnough is null
                    ? transparent
                    : GetIndexFromColor(closeEnough.Color);

    }

    public static ColorName? FindTheClosestColor(List<ColorName>     colorNames
                                               , MauiGraphics.Color? targetColor)
    {
        var minDistance = double.MaxValue;

        ColorName? closestColor = null;

        foreach (var colorName in colorNames)
        {
            var distance = CalculateColorDistance(targetColor
                                                , colorName.Color);

            if ( ! (distance < minDistance)) continue;

            minDistance  = distance;
            closestColor = colorName;
        }

        return closestColor;

    }

    /// <summary>
    /// Method to calculate the Euclidean distance between two colors in RGB space
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    public static double CalculateColorDistance(MauiGraphics.Color? color1
                                              , MauiGraphics.Color? color2)
    {
        if (color1 is null || color2 is null) throw new NullReferenceException("'color1' or 'color2', or both, are null.");

        double redDiff   = color1.Red - color2.Red;
        double greenDiff = color1.Green - color2.Green;
        double blueDiff  = color1.Blue - color2.Blue;

        redDiff   = Math.Abs(redDiff);
        greenDiff = Math.Abs(greenDiff);
        blueDiff  = Math.Abs(blueDiff);

        var results = Math.Sqrt(redDiff * redDiff
                              + greenDiff * greenDiff
                              + blueDiff * blueDiff);
#if DEBUG
        var debugInfo = new StringBuilder();
        debugInfo.AppendLine($"redDif = color1.Red ({color1.Red}) - color2.Red ({color2.Red}): {redDiff}");
        debugInfo.AppendLine($"greenDiff = color1.Green ({color1.Green}) - color2.Green ({color2.Green}): {greenDiff}");
        debugInfo.AppendLine($"blueDiff = color1.Blue ({color1.Blue}) - color2.Blue ({color2.Blue}): {blueDiff}");
        debugInfo.AppendLine($"results = Math.Sqrt( {redDiff} * {redDiff} + {greenDiff} * {greenDiff} + {blueDiff} * {blueDiff} ): {results} ");
        var infoAsString = debugInfo.ToString();
#endif

        return results;
    }

    public static void PopulateColorNames()
    {
        if (ColorNames is not null && ColorNames.Count > 0) return;

        ColorNames = new ObservableCollection<ColorName>();

        var systemColors = GetNamedColors();
        foreach (var systemColor in systemColors)
        {
            ColorNames.Add(new ColorName
                           {
                               Name  = systemColor
                             , Color = ConvertSystemColorNameToMauiColor(systemColor)
                           });
        }
    }

    public static void ApplyDefaultColors()
    {
        var mergeDictionaries       = Application.Current?.Resources.MergedDictionaries ?? new List<ResourceDictionary>();
        var dictionaryWithPstColors = new ResourceDictionary();

        foreach (var dictionary in mergeDictionaries)
        {
            if (dictionary.All(dict => dict.Key
                                    != ResourceColors.Primary.ToString()))
            {
                continue;
            }

            dictionaryWithPstColors = dictionary;

            break;
        }

        PreferencesDataStore.DefaultPrimaryColorName = dictionaryWithPstColors[ResourceColors.Primary
                                                                                             .ToString()].ToString();
        PreferencesDataStore.DefaultSecondaryColorName = Application.Current
                                                                    ?.Resources[ResourceColors.Secondary
                                                                                              .ToString()]
                                                                    .ToString();
        PreferencesDataStore.DefaultTertiaryColorName = Application.Current
                                                                   ?.Resources[ResourceColors.Tertiary
                                                                                             .ToString()]
                                                                   .ToString();
    }

    //BENDO: Fix this method based on this way of calculating the contrast ratio.
    //This uses "Red" and "Green" as inputs
    /*
     * Relative Luminance = 0.2126 × (0.50196) + 0.7152 × (0.50196) + 0.0722 × (1.0)
       Given that Red is (255, 0, 0) and Green is (0, 255, 0):
       For Red:
            Relative Luminance[Red] = 0.2126 × (1) + 0.7152 × (0) + 0.0722 × (0)
            Relative Luminance[Red] = 0.2126

       For Green:
            Relative Luminance[Green] = 0.2126 × (0) + 0.7152 × (1) + 0.0722 × (0)
            Relative Luminance[Green] = 0.7152

       Now, we can calculate the contrast ratio using the formula:
            Contrast Ratio = (max(Relative Luminance[Red], Relative Luminance[Green]) + 0.05)
                           / (min(Relative Luminance[Red], Relative Luminance[Green]) + 0.05)

       Substituting in the values:
            Contrast Ratio = (max(0.2126, 0.7152) + 0.05)
                           / (min(0.2126, 0.7152) + 0.05)
            Contrast Ratio = (0.7152 + 0.05)
                           / (0.2126) + 0.05)
            Contrast Ratio ≈ 0.8152
                           / 0.3126
            Contrast Ratio ≈ 2.61
     */
    public static double GetContrastRatio(Color backgroundColor
                                        , Color textColor)
    {
        var backLuminance = GetRelativeLuminance(backgroundColor);
        var textLuminance = GetRelativeLuminance(textColor);

        var contrastRatio = (Math.Max(backLuminance, textLuminance) + 0.05)
                          / (Math.Min(backLuminance, textLuminance) + 0.05);

        return contrastRatio;
    }

    /// <summary>
    /// The formula used to calculate relative luminance is based on the sRGB color space standard and
    /// is defined by the Web Content Accessibility Guidelines (WCAG).
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static double GetRelativeLuminance(Color color)
    {
        const double rWeight                          = 0.2126;
        const double gWeight                          = 0.7152;
        const double bWeight                          = 0.0722;
        const double thresholdStart                   = 0.005;
        const double thresholdEnd                     = 0.04045;
        const double linearThreshold                  = 12.92;
        const double gammaCorrection                  = 2.4;
        const double gammaCorrectionForBrighterColors = 1.055;

        var red   = color.R / 255.0;
        var green = color.G / 255.0;
        var blue  = color.B / 255.0;

        red = red <= thresholdStart
            ? red / linearThreshold
            : Math.Pow((red + thresholdEnd) / gammaCorrectionForBrighterColors
                     , gammaCorrection);

        green = green <= thresholdStart
            ? green / linearThreshold
            : Math.Pow((green + thresholdEnd) / gammaCorrectionForBrighterColors
                     , gammaCorrection);

        blue = blue <= thresholdStart
            ? blue / linearThreshold
            : Math.Pow((blue + thresholdEnd) / gammaCorrectionForBrighterColors
                     , gammaCorrection);

        return rWeight * red
             + gWeight * green
             + bWeight * blue;
    }

    private static double GetRelativeLuminance(MauiGraphics.Color? color)
    {
        var systemColor = ConvertMauiColorToSystemColor(color);

        return GetRelativeLuminance(systemColor);
    }

    public static MauiGraphics.Color? ChooseReadableTextColor(MauiGraphics.Color? backgroundColor)
    {
        const double yellowThreshold = 0.2;
        const double lightThreshold = 0.5;

        var systemBackColor = ConvertMauiColorToSystemColor(backgroundColor);

        var black  = MauiGraphics.Color.FromArgb(ColorInfo.BlackAsHex);
        var white  = MauiGraphics.Color.FromArgb(ColorInfo.WhiteAsHex);
        var yellow = MauiGraphics.Color.FromArgb(ColorInfo.YellowAsHex);

        var bgLuminance = GetRelativeLuminance(systemBackColor);

        var isLightBackground = bgLuminance > lightThreshold;

        if (isLightBackground) return black;

        return bgLuminance < yellowThreshold
                    ? white
                    : yellow;
    }
}

public enum ResourceColors
{
    Primary
  , Secondary
  , Tertiary
  , InvalidColor //only used in unit test
}
