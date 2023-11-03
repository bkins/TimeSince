using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace XamarinUtilities;

public class ColorUtility
{
    // Method to get color hex from color name
    public static string GetHexFromName(string colorName)
    {
        // Convert the color name to a Color object
        //Color color = Color.FromName(colorName);

        // Convert the Color object to its hexadecimal representation
       // string hexColor = $"#{(int)(color.R * 255):X2}{(int)(color.G * 255):X2}{(int)(color.B * 255):X2}";

       // return hexColor;
       return string.Empty;
    }

    // Method to get color name from hex
    public static string GetNameFromHex(string hexColor)
    {
        // Remove the '#' symbol if present
        hexColor = hexColor.TrimStart('#');

        // Parse the hexadecimal string into RGB components
        byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // Create a Color object from the RGB components
        //Color color = new Color(r / 255.0, g / 255.0, b / 255.0);

        // Get the closest color name for the Color object
        //string colorName = color.ToName();

        //return colorName;
        return string.Empty;
    }

    public static List<string> GetColorNames()
    {
        // Use reflection to get the properties representing colors in Xamarin.Forms.Color
        PropertyInfo[] colorProperties = typeof(Xamarin.Forms.Color).GetProperties(BindingFlags.Public
                                                                                      | BindingFlags.Static);

        // Filter the properties to select only Color properties
        var colorNames = colorProperties
                         .Where(p => p.PropertyType == typeof(Color))
                         .Select(p => p.Name)
                         .ToList();

        return colorNames;
    }

    public static Color GetColorFromName(string colorName)
    {
        return (Color)typeof(Color)?.GetRuntimeProperty(colorName)?.GetValue(null)!;
    }
}
