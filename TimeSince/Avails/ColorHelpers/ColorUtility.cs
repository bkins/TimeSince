using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using TimeSince.Data;
using TimeSince.MVVM.Models;
using MauiGraphics = Microsoft.Maui.Graphics;
using Color = System.Drawing.Color;

namespace TimeSince.Avails.ColorHelpers;

/// <summary>
/// Provides utility methods for working with colors in the context of MAUI applications.
/// </summary>
public static class ColorUtility
{
    /// <summary>
    /// Gets or sets the list of color names.
    /// </summary>
    public static ObservableCollection<ColorName>? ColorNames { get; private set; }

    /// <summary>
    /// Returns a list of color names that are present in the <see cref="Color"/> type.
    /// </summary>
    /// <returns>A list of color names.</returns>
    public static List<string> GetListOfNamedColors()
    {
        return typeof(Color).GetProperties(BindingFlags.Public
                                         | BindingFlags.Static)
                            .Where(propertyInfo => propertyInfo.PropertyType == typeof(Color))
                            .Select(propertyInfo => propertyInfo.Name)
                            .ToList();
    }

    /// <summary>
    /// Converts the System color name to a MAUI color.
    /// </summary>
    /// <param name="systemColorName">The System color name to convert.</param>
    /// <param name="defaultColor">The color to return if a System color is not found.</param>
    /// <returns>The MAUI color corresponding to the System color name.</returns>
    public static MauiGraphics.Color? ConvertSystemColorNameToMauiColor(string? systemColorName
                                                                      , Color? defaultColor = null)
    {
        var systemColor = GetColorFromName(systemColorName
                                         , defaultColor);

        return ConvertSystemColorToMauiColor(systemColor);
    }

    /// <summary>
    /// Converts the Maui color to the corresponding System color.
    /// </summary>
    /// <param name="mauiColor">The Maui color to convert.</param>
    /// <returns>The System color corresponding to the Maui color.</returns>
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

    /// <summary>
    /// Sets the resource colors based on user preferences (<see cref="Preferences"/>).
    /// </summary>
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

    /// <summary>
    /// Sets the default resource colors in the absence of user preferences (<see cref="Preferences"/>).
    /// </summary>
    public static void SetDefaultResourceColors()
    {
        UpdateColorResource(ResourceColors.Primary
                          , MauiGraphics.Color.FromArgb(ColorInfo.RoyalAzureAsHex));
        UpdateColorResource(ResourceColors.Secondary
                          , MauiGraphics.Color.FromArgb(ColorInfo.LavenderMistAsHex));
        UpdateColorResource(ResourceColors.Tertiary
                          , MauiGraphics.Color.FromArgb(ColorInfo.MidnightIndigoAsHex));

    }

    /// <summary>
    /// Updates the specified resource color with the provided Maui color.
    /// </summary>
    /// <param name="resourceColorName">The resource color to update.</param>
    /// <param name="mauiColor">The Maui color to set.</param>
    /// <returns>True if the update is successful, false otherwise.</returns>
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
                          , GetColorInListOfColors(mauiColor
                                           , (ColorNames ?? []).ToList()));
        }
        catch (Exception e)
        {
            App.Logger.LogError(e);

            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the Maui color associated with the specified resource color.
    /// </summary>
    /// <param name="resourceColorsName">The resource color to retrieve.</param>
    /// <returns>The Maui color associated with the resource color.</returns>
    public static MauiGraphics.Color? GetColorFromResources(ResourceColors resourceColorsName)
    {
        try
        {
            object? color = null;
            if ((bool)Application.Current?.Resources.TryGetValue(resourceColorsName.ToString(), out color))
            {
                return color as MauiGraphics.Color;
            }
        }
        catch (Exception e)
        {
            return HandleConversionError(resourceColorsName, e);
        }

        return null;
    }

    /// <summary>
    /// Gets the index of a color in the list based on a partial color name.
    /// </summary>
    /// <param name="partialName">The partial name used to search for a color in the list.</param>
    /// <returns>
    /// The index of the color in the list. Returns -1 if the partial name is null, or if no color
    /// with a name starting with the specified partial name is found in the list.
    /// </returns>
    public static int GetIndexFromPartialName(string? partialName)
    {
        if (partialName is null || ColorNames is null) return -1;

        return ColorNames.ToList()
                         .FindIndex(colorName => colorName.Name!
                                                          .StartsWith(partialName.ToLower()
                                                                    , StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Gets the color name associated with the provided Maui color from a list of color names.
    /// </summary>
    /// <param name="mauiColor">The Maui color to find in the list.</param>
    /// <param name="colorNames">The list of color names to search.</param>
    /// <returns>
    /// The name of the color associated with the Maui color. Returns an empty string if the Maui color
    /// is null or if no exact match is found. If no exact match is found, it attempts to find a close
    /// match in the list and returns the name of the closest matching color. Returns an empty string if
    /// no close match is found.
    /// </returns>
    public static string? GetColorInListOfColors(MauiGraphics.Color? mauiColor
                                               , List<ColorName>     colorNames)
    {
        if (mauiColor is null) return string.Empty;

        var colorName = colorNames.FirstOrDefault(name => name.Color
                                                              ?.ToArgbHex()
                                                       == mauiColor.ToArgbHex());

        //Color was found, return it
        if (colorName is not null) return colorName.Name;

        //Find a color that is in the list that is a close match
        var closeEnough = FindTheClosestColor(colorNames
                                            , mauiColor);

        //A close match could not be found, just return an empty string
        if (closeEnough is null) return string.Empty;

        //See if the closest match is in the list
        return GetColorInListOfColors(closeEnough.Color
                                    , colorNames);
    }

    /// <summary>
    /// Gets the index of the provided Maui color in the list of color names.
    /// </summary>
    /// <param name="mauiColor">The Maui <see cref="Microsoft.Maui.Graphics.Color"/> to find in the list.</param>
    /// <returns>
    /// The index of the Maui color in the list. Returns the index of the "Transparent" color
    /// if the Maui color is null. If the Maui color is not found in the list, it finds the
    /// closest matching color and returns its index. If no match is found, it returns the
    /// index of the "Transparent" color.
    /// </returns>
    public static int GetIndexFromColor(MauiGraphics.Color? mauiColor)
    {
        var colorNamesList = (ColorNames ?? []).ToList();

        var transparent = colorNamesList.FindIndex(name => name.Name == "Transparent");

        //If parameter `mauiColor` is null return transparent as the default value.
        if (mauiColor is null) return transparent;

        var index = colorNamesList.FindIndex(name => name.Color?.ToArgbHex() == mauiColor.ToArgbHex());

        //Color was found, return it.
        if (index != -1) return index;

        //Find the closest match to the color
        var closeEnough = FindTheClosestColor(colorNamesList
                                            , mauiColor);

        //If no match was found return transparent
        //, otherwise search again for the closest matched color in the list.
        return closeEnough is null
                    ? transparent
                    : GetIndexFromColor(closeEnough.Color);

    }

    /// <summary>
    /// Finds the closest matching color in the list to the provided Maui color.
    /// </summary>
    /// <param name="colorNames">The list of color names to search.</param>
    /// <param name="targetColor">The target Maui <see cref="Microsoft.Maui.Graphics.Color"/>
    /// for which to find the closest match.</param>
    /// <returns>
    /// The closest matching color name from the list based on Euclidean distance in RGB space.
    /// Returns null if the list is empty or if an error occurs during the calculation.
    /// </returns>
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

    /// <summary>
    /// Populates the list of color names (<see cref="ColorNames"/>) if it is not already populated.
    /// </summary>
    public static bool PopulateColorNames()
    {
        if (ColorNames is not null && ColorNames.Count > 0) return true;

        ColorNames = new ObservableCollection<ColorName>();

        var systemColors = GetListOfNamedColors();
        foreach (var systemColor in systemColors)
        {
            ColorNames.Add(new ColorName
                           {
                               Name  = systemColor
                             , Color = ConvertSystemColorNameToMauiColor(systemColor)
                           });
        }

        return ColorNames.Count > 0;
    }

    /// <summary>
    /// Applies default colors to the user's preferences (<see cref="Preferences"/>)
    /// based on the current merged dictionaries.
    /// </summary>
    public static void ApplyDefaultColors()
    {
        var mergeDictionaries = Application.Current
                                           ?.Resources
                                           .MergedDictionaries
                             ?? new List<ResourceDictionary>();

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

    /// <summary>
    /// Calculates the contrast ratio between two colors.
    /// </summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="textColor">The text color.</param>
    /// <returns>The calculated contrast ratio.</returns>
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
    /// Calculates the relative luminance of a <see cref="System.Drawing.Color"/>
    /// based on the sRGB color space standard defined by the Web Content Accessibility Guidelines (WCAG).
    /// </summary>
    /// <param name="color">The <see cref="System.Drawing.Color"/> for which to calculate relative
    /// luminance.</param>
    /// <returns>The relative luminance of the <see cref="System.Drawing.Color"/>.</returns>
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

    /// <summary>
    /// Calculates the relative luminance of a <see cref="Microsoft.Maui.Graphics.Color"/>
    /// based on the sRGB color space standard defined by the Web Content Accessibility Guidelines (WCAG).
    /// To accomplish this, it first converts the MAUI <see cref="Microsoft.Maui.Graphics.Color"/> to
    /// a System.Drawing.<see cref="System.Drawing.Color"/>, then calls <see cref="GetRelativeLuminance"/>
    /// </summary>
    /// <param name="color">The <see cref="Microsoft.Maui.Graphics.Color"/> for which to calculate relative
    /// luminance.</param>
    /// <returns>The relative luminance of the <see cref="Microsoft.Maui.Graphics.Color"/>.</returns>
    public static double GetRelativeLuminance(MauiGraphics.Color? color)
    {
        var systemColor = ConvertMauiColorToSystemColor(color);

        return GetRelativeLuminance(systemColor);
    }

    /// <summary>
    /// Chooses a readable text color based on the background color.
    /// </summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <returns>The chosen readable text color.</returns>
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

#region Private Methods

    private static MauiGraphics.Color? HandleConversionError(ResourceColors resourceColorsName
                                                           , Exception      e)
    {
        App.Logger.LogError("Error while getting color from Resources.  Converting System color to Maui color."
                          , e.Message
                          , e.StackTrace ?? string.Empty);

        if (e is InvalidCastException)
        {
            return ConvertSystemColorToMauiColor((Color)Application.Current
                                                                   ?.Resources[resourceColorsName.ToString()]!);
        }

        return null;
    }

    /// <summary>
    /// Gets a color from its name. i.e. Converts a string to a nullable <see cref="Color"/>
    /// </summary>
    /// <param name="colorName">The name of the color.</param>
    /// <param name="defaultColor">The default color to return if the specified color name is not found.</param>
    /// <returns>The color corresponding to the name.</returns>
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

    /// <summary>
    /// Converts a System.Drawing.<see cref="System.Drawing.Color"/> to its
    /// equivalent MAUI <see cref="Microsoft.Maui.Graphics.Color"/> representation.
    /// </summary>
    /// <param name="systemColor">The System.Drawing.<see cref="System.Drawing.Color"/> to convert.</param>
    /// <returns>
    /// The equivalent MAUI <see cref="Microsoft.Maui.Graphics.Color"/> representation of the
    /// provided System.Drawing.<see cref="System.Drawing.Color"/>.
    /// Returns null if the input color is null.
    /// </returns>
    private static MauiGraphics.Color? ConvertSystemColorToMauiColor(Color? systemColor)
    {
        if (systemColor is null) return null;

        return new MauiGraphics.Color(systemColor?.R ?? 0
                                    , systemColor?.G ?? 0
                                    , systemColor?.B ?? 0
                                    , systemColor?.A ?? 0);
    }

    /// <summary>
    /// Updates the color name in the preferences (<see cref="Preferences"/>) based on the specified resource color.
    /// </summary>
    /// <param name="resourceColorName">The resource color for which to update the name.</param>
    /// <param name="name">The new color name to be set in preferences.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when an unsupported or invalid <paramref name="resourceColorName"/> is provided.
    /// </exception>
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

#endregion

}

/// <summary>
/// Enumeration of resource colors used in the application.
/// </summary>
public enum ResourceColors
{
    Primary
  , Secondary
  , Tertiary
  , InvalidColor //only used in unit tests
}
