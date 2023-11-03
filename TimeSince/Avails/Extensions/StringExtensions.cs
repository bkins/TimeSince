using System.Globalization;
using System.Text.RegularExpressions;
using static System.StringComparison;

namespace TimeSince.Avails.Extensions;

public static class StringExtensions
{
    private const string BadTimeFormatMessage = "To convert to a time, the value must be a whole number or in [hh:]mm[:ss] format.";

    public static bool IsNullEmptyOrWhitespace(this string value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// "HasValue" means the string is:
    ///     NOT Null,
    /// and NOT Empty,
    /// and NOT Whitespace
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasValue(this string value)
    {
        return ! IsNullEmptyOrWhitespace(value);
    }

    public static string ToTitleCase(this string value
                                   , bool        force)
    {
        var ti = new CultureInfo("en-US"
                               , false).TextInfo;

        if (force)
        {
            value = value?.ToLower();
        }
        return ti.ToTitleCase(value);
    }

    public static string ToTitleCase(this string value)
    {
        var textInfo = new CultureInfo("en-US"
                                     , false).TextInfo;
        return textInfo.ToTitleCase(nameof(value));
    }

    /// <summary>
    /// Converts a string in either HH:MM:SS or MM:SS formats to a TimeSpan
    /// </summary>
    /// <param name="timeAsString"></param>
    /// <returns></returns>
    /// <exception cref="FormatException">If the string does not contain ':' or has more than two ':'.</exception>
    public static TimeSpan ToTimeSpan(this string timeAsString)
    {
        //BENDO: Refactor this method

        //No value, return a new TimeSpan
        if (timeAsString.IsNullEmptyOrWhitespace())
        {
            return new TimeSpan(0, 0, 0, 0);
        }

        //Value is a whole number, return TimeSpan in minutes
        if (int.TryParse(timeAsString
                       , out var number))
        {
            return new TimeSpan(0, 0, number, 0);
        }

        //Value is not in a time format (there is no ':' in string), throw error
        if ( ! timeAsString.Contains(":"))
            throw new FormatException(BadTimeFormatMessage);

        var timeParts = timeAsString.Split(':');

        return timeParts.Length switch
               {
                   2 => //00:00
                       TryToGetTimeInMinutesAndSeconds(timeParts)
                 , 3 => //00:00:00
                       TryToGetTimeInHoursMinutesAndSeconds(timeParts)
                 , _ => throw new FormatException(BadTimeFormatMessage)
               };
    }

    private static TimeSpan TryToGetTimeInHoursMinutesAndSeconds(IReadOnlyList<string> timeParts)
    {
        TimeSpan time = default;

        if (int.TryParse(timeParts[0], out var hours)
         && int.TryParse(timeParts[1], out var minutes)
         && int.TryParse(timeParts[2], out var seconds))
        {
            time = new TimeSpan(0, hours, minutes, seconds);
        }

        return time;
    }

    private static TimeSpan TryToGetTimeInMinutesAndSeconds(IReadOnlyList<string> timeParts)
    {
        TimeSpan time = default;

        if (int.TryParse(timeParts[0], out var minutes)
         && int.TryParse(timeParts[1], out var seconds))
        {
            time = new TimeSpan(0, 0, minutes, seconds);
        }

        return time;
    }

    /// <summary>
    /// Converts a TimeSpan to a HH:MM:SS or MM:SS (if hours are zero) string.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns>A string representing the TimeSpan in HH:MM:SS or MM:SS (if hours are zero) format.</returns>
    public static string ToShortForm(this TimeSpan timeSpan)
    {
        var shortForm = "";

        if (timeSpan.Hours > 0)
        {
            shortForm += $"{timeSpan.Hours}:";
        }

        //Add leading zeroes to minutes and seconds
        shortForm += $"{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";

        return shortForm;
    }

    public static bool IsTrue(this string value)
    {
        return value.Equals("yes",  CurrentCultureIgnoreCase)
            || value.Equals("y",    CurrentCultureIgnoreCase)
            || value.Equals("true", CurrentCultureIgnoreCase)
            || value.Equals("t",    CurrentCultureIgnoreCase);
    }

    public static int ToSafeInt(this string value)
    {
        return int.TryParse(value, out var outInt) ?
                        outInt
                        : 0;
    }

    public static double ToSafeDouble(this string value)
    {
        return double.TryParse(value, out var outDouble)
                        ? outDouble
                        : 0.0;
    }

    public static bool SafeContains(this string value, string searchValue)
    {
        if (searchValue is null)
        {
            return true;
        }

        return value != null && value.Contains(searchValue);
    }

    public static bool Contains(this string source, string toCheck, StringComparison stringComparer)
    {
        return source?.IndexOf(toCheck, stringComparer) >= 0;
    }

    public static bool NotContains(this string value
                                 , string      toCheck)
    {
        return ! value.Contains(toCheck);
    }

    public static string SplitCamelCase(this string value)
    {
        return Regex.Replace(Regex.Replace(value, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                             @"(\p{Ll})(\P{Ll})",
                             "$1 $2");
    }

}
