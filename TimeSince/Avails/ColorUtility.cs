using System.Collections.ObjectModel;
using System.Reflection;
using TimeSince.Avails.Extensions;
using TimeSince.Data;
using TimeSince.MVVM.Models;
using MauiColor = Microsoft.Maui.Graphics;
using Color = System.Drawing.Color;

namespace TimeSince.Avails;

public static class ColorUtility
{
    public static ObservableCollection<ColorName> ColorNames { get; set; }
    public static List<string> GetNamedColors()
    {
        var namedColors = typeof(Color)
                          .GetProperties(BindingFlags.Public
                                       | BindingFlags.Static)
                          .Where(p => p.PropertyType == typeof(Color))
                          .Select(p => p.Name)
                          .ToList();

        return namedColors;
    }

    public static Microsoft.Maui.Graphics.Color ConvertSystemColorNameToMauiColor(string systemColorName)
    {
        var systemColor = GetColorFromName(systemColorName);

        return ConvertSystemColorToMauiColor(systemColor);
    }

    public static Color? GetColorFromName(string colorName)
    {
        if (colorName is null) return null;

        var colorProperty = typeof(Color).GetProperty(colorName
                                                    , BindingFlags.Public
                                                    | BindingFlags.Static);

        if (colorProperty != null && colorProperty.PropertyType == typeof(Color))
        {
            return (Color)colorProperty.GetValue(null)!;
        }

        // Handle the case where the color name is not found or invalid
        // You can return a default color or throw an exception as needed
        return null; // Default color (or another choice)
    }

    public static Microsoft.Maui.Graphics.Color ConvertSystemColorToMauiColor(Color? systemColor)
    {
        if (systemColor is null) return null;

        return new MauiColor.Color(systemColor?.R ?? 0
                                 , systemColor?.G ?? 0
                                 , systemColor?.B ?? 0
                                 , systemColor?.A ?? 0);
    }

    public static void SetResourceColors()
    {
        UpdateColorResource(ResourceColors.Primary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.PrimaryColorName)));
        UpdateColorResource(ResourceColors.Secondary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.SecondaryColorName)));
        UpdateColorResource(ResourceColors.Tertiary
                          , ConvertSystemColorToMauiColor(GetColorFromName(PreferencesDataStore.TertiaryColorName)));

        // SetResourceColor(ResourceColors.Primary.ToString(), PreferencesDataStore.PrimaryColorName);
        // SetResourceColor(ResourceColors.Secondary.ToString(), PreferencesDataStore.SecondaryColorName);
        // SetResourceColor(ResourceColors.Tertiary.ToString(), PreferencesDataStore.TertiaryColorName);
    }

    public static void SetDefaultResourceColors()
    {
        UpdateColorResource(ResourceColors.Primary
                          , Microsoft.Maui.Graphics.Color.FromArgb("#512BD4"));
        UpdateColorResource(ResourceColors.Secondary
                          , Microsoft.Maui.Graphics.Color.FromArgb("#DFD8F7"));
        UpdateColorResource(ResourceColors.Tertiary
                          , Microsoft.Maui.Graphics.Color.FromArgb("#2B0B98"));

    }
    private static void SetResourceColor(string resourceColorName
                                       , string colorName)
    {
        if (colorName.IsNullEmptyOrWhitespace()) return;

        if ( ! Application.Current?.Resources.TryGetValue(resourceColorName
                                                        , out _) ?? true) return;
        Application.Current
                   .Resources[resourceColorName] = GetColorFromName(colorName);
    }

    public static bool UpdateColorResource(ResourceColors                resourceColorName
                                       , Microsoft.Maui.Graphics.Color mauiColor)
    {
        if (mauiColor is null) return false; // Is true more appropriate?

        if (Application.Current == null) return false;

        var appResources = Application.Current.Resources;
        var colorName    = resourceColorName.ToString();

        if (! appResources.TryGetValue(colorName
                                     , out _)) return false;

        appResources[colorName] = mauiColor;

        UpdateColorName(resourceColorName, GetNameFromColor(mauiColor, ColorNames.ToList()));

        return true;
    }

    private static void UpdateColorName(ResourceColors resourceColorName, string name)
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

            default:
                throw new ArgumentOutOfRangeException(nameof(resourceColorName)
                                                    , resourceColorName
                                                    , null);
        }
    }

    public static Microsoft.Maui.Graphics.Color GetColorFromResources(ResourceColors resourceColorsName)
    {
        try
        {
            if (Application.Current
                           ?.Resources
                           .TryGetValue(resourceColorsName.ToString()
                                      , out _)
             ?? false)
            {
                return (MauiColor.Color)Application.Current
                                                   ?.Resources[resourceColorsName.ToString()];
            }
        }
        catch (InvalidCastException e)
        {
            return ConvertSystemColorToMauiColor((Color)Application.Current
                                                                   ?.Resources[resourceColorsName.ToString()]!);
        }
        catch (Exception e)
        {

            Console.WriteLine(e);

            return null;
        }

        return null;
    }

    public static int GetIndexFromPartialName(string partialName)
    {
        var index = ColorNames.ToList().FindIndex(colorName => colorName.Name
                                                               .StartsWith(partialName.ToLower()
                                                                         , StringComparison.CurrentCultureIgnoreCase));

        return index;
    }

    public static string GetNameFromColor(MauiColor.Color mauiColor
                                        , List<ColorName> colorNames)
    {
        if (mauiColor is null) return string.Empty;

        var colorName = colorNames.FirstOrDefault(name => name.Color.ToArgbHex()== mauiColor.ToArgbHex());

        if (colorName is not null) return colorName.Name;

        var closeEnough = FindTheClosestColor(colorNames
                                            , mauiColor);

        if (closeEnough is null) return string.Empty;

        return GetNameFromColor(closeEnough.Color
                              , colorNames);
    }

    public static int GetIndexFromColor(MauiColor.Color mauiColor)
    {
        if (mauiColor is null) return 0;

        var colorNamesList = ColorNames.ToList();
        var index          = colorNamesList.FindIndex(name => name.Color.ToArgbHex() == mauiColor.ToArgbHex());

        if (index != -1) return index;

        var closeEnough = FindTheClosestColor(colorNamesList
                                            , mauiColor);

        if (closeEnough is null) return 0;

        return GetIndexFromColor(closeEnough.Color);
    }

    private static ColorName FindTheClosestColor(List<ColorName> colorNames, MauiColor.Color targetColor)
    {
        var minDistance = double.MaxValue;

        ColorName closestColor = null;

        foreach (var colorName in colorNames)
        {
            double distance = CalculateColorDistance(targetColor, colorName.Color);

            if (distance < minDistance)
            {
                minDistance  = distance;
                closestColor = colorName;
            }
        }

        return closestColor;

    }
    /// <summary>
    /// Method to calculate the Euclidean distance between two colors in RGB space
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    private static double CalculateColorDistance(MauiColor.Color color1, MauiColor.Color color2)
    {
        double rDiff = color1.Red - color2.Red;
        double gDiff = color1.Green - color2.Green;
        double bDiff = color1.Blue - color2.Blue;
        return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
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

        PreferencesDataStore.DefaultPrimaryColorName = dictionaryWithPstColors[ResourceColors.Primary.ToString()]
                                                                  .ToString();
        PreferencesDataStore.DefaultSecondaryColorName = Application.Current
                                                                  ?.Resources[ResourceColors.Secondary.ToString()]
                                                                  .ToString();
        PreferencesDataStore.DefaultTertiaryColorName = Application.Current
                                                                  ?.Resources[ResourceColors.Tertiary.ToString()]
                                                                  .ToString();
    }
}

public enum ResourceColors
{
    Primary
  , Secondary
  , Tertiary
}
