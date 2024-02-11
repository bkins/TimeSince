using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using static System.StringComparison;

namespace TimeSince.Avails.Extensions;

/// <summary>
/// String extension methods for various operations.
/// </summary>
public static partial class StringExtensions
{
    private const string BadTimeFormatMessage = "To convert to a time, the value must be a whole number or in [hh:]mm[:ss] format.";

    /// <summary>
    /// Checks if the string is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">Is checked for if it is null, empty, or whitespace.</param>
    /// <returns>Boolean value based on if the string is null, empty, or whitespace.</returns>
    public static bool IsNullEmptyOrWhitespace(this string? value)
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
    public static bool HasValue(this string? value)
    {
        return ! IsNullEmptyOrWhitespace(value);
    }

    /// <summary>
    /// Converts a string to title case.
    /// e.g. If "Converts a string to title case." passed in, it would return:
    /// "Converts A String To Title Case."
    /// </summary>
    /// <param name="value"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string? value
                                   , bool         force)
    {
        var cultureTextInfo = new CultureInfo("en-US"
                                            , false).TextInfo;

        if (force)
        {
            value = value?.ToLower();
        }
        return cultureTextInfo.ToTitleCase(value ?? string.Empty);
    }

    /// <summary>
    /// Converts a string in either HH:MM:SS or MM:SS formats to a TimeSpan
    /// </summary>
    /// <param name="timeAsString"></param>
    /// <returns></returns>
    /// <exception cref="FormatException">If the string does not contain ':' or has more than two ':'.</exception>
    public static TimeSpan ToTimeSpan(this string timeAsString)
    {
        // Validate the input string and return default value if needed
        if ( ! IsValidTimeString(timeAsString, out var defaultValue)) return defaultValue;

        // Split the time string by ':' and determine the format based on the number of parts
        var timeParts = timeAsString.Split(':');

        return timeParts.Length switch
               {
                   2 => TryToGetTimeInMinutesAndSeconds(timeParts)       //00:00
                 , 3 => TryToGetTimeInHoursMinutesAndSeconds(timeParts)  //00:00:00
                 , _ => throw new FormatException(BadTimeFormatMessage)
               };
    }

    private static bool IsValidTimeString(string timeAsString, out TimeSpan defaultValue)
    {
        defaultValue = new TimeSpan(0, 0, 0, 0);

        // No value, return a new TimeSpan
        if (timeAsString.IsNullEmptyOrWhitespace()) return false;

        // Value is a whole number, return TimeSpan in minutes
        if ( ! int.TryParse(timeAsString, out var number)) return timeAsString.Contains(':');

        defaultValue = new TimeSpan(0, 0, number, 0);
        return false;
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
    /// Converts a TimeSpan to a HH:mm:ss or MM:SS (if hours are zero) string.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns>A string representing the TimeSpan in HH:mm:ss or mm:ss (if hours are zero) format.</returns>
    public static string ToShortForm(this TimeSpan timeSpan)
    {
        var shortForm = "";

        if (timeSpan.Hours > 0)
        {
            shortForm += $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:";
        }

        //Add leading zeroes to minutes and seconds
        shortForm += $"{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";

        return shortForm;
    }

    /// <summary>
    /// Checks if the string represents a truth value.
    /// False:
    /// Value is null, OR
    /// "0"
    /// True:
    /// "yes", OR
    /// "y", OR
    /// "true", OR
    /// "t"
    /// (all string comparisons above done using <see cref="StringComparison.CurrentCultureIgnoreCase"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsTrue(this string value)
    {
        if (value is null or "0") return false;

        return value.Equals("yes",  CurrentCultureIgnoreCase)
            || value.Equals("y",    CurrentCultureIgnoreCase)
            || value.Equals("true", CurrentCultureIgnoreCase)
            || value.Equals("t",    CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Converts the string to a safe integer.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToSafeInt(this string value)
    {
        if (double.TryParse(value, out var doubleValue))
        {
            return (int)doubleValue;
        }

        return 0;
    }

    /// <summary>
    /// Converts the string to a safe double.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double ToSafeDouble(this string value)
    {
        return double.TryParse(value, out var outDouble)
                        ? outDouble
                        : 0.0;
    }

    /// <summary>
    /// Checks if the string contains another string, with optional case-insensitivity.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="searchValue"></param>
    /// <param name="caseInsensitive"></param>
    /// <returns></returns>
    public static bool SafeContains(this string? value
                                  , string? searchValue
                                  , bool caseInsensitive = false)
    {
        switch (value)
        {
            case null when searchValue is null:
                return true;
            case null:
                return false;
        }

        if (searchValue is null) return true;

        return caseInsensitive
                    ? value.Contains(searchValue, CurrentCultureIgnoreCase)
                    : value.Contains(searchValue);

    }

    /// <summary>
    /// Checks if the string does not contain another string, with optional case-insensitivity.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="searchValue"></param>
    /// <param name="caseInsensitive"></param>
    /// <returns></returns>
    public static bool NotContains(this string? value
                                 , string?      searchValue
                                 , bool         caseInsensitive = false)
    {
        return ! SafeContains(value
                            , searchValue
                            , caseInsensitive);
    }

    /// <summary>
    /// Insert spaces between words in a given string, where a word is defined as a sequence of lowercase letters.
    /// </summary>
    /// <param name="value">The string that is to be split into words.</param>
    /// <returns>A string with all words separated by spaces.</returns>
    /// <remarks>Words in all caps (usually Acronyms) are treated as a word. E.g. "AAATest" is two words, and will return "AAA Test"</remarks>
    public static string SplitCamelCase(this string value)
    {
        return value.IsNullEmptyOrWhitespace()
                        ? value
                        : LowerCaseToNonLowercase().Replace(NonLowercaseToLowerCase().Replace(value, "$1 $2"), "$1 $2");

    }

    [GeneratedRegex(@"(\p{Ll})(\P{Ll})")]
    private static partial Regex LowerCaseToNonLowercase();

    [GeneratedRegex(@"(\P{Ll})(\P{Ll}\p{Ll})")]
    private static partial Regex NonLowercaseToLowerCase();

    /// <summary>
    /// Checks if the string is a valid JSON format.
    /// </summary>
    /// <param name="json"></param>
    /// <returns>Returns true if the string is valid JSON; otherwise, false.</returns>
    public static bool IsValidJson(this string json)
    {
        try
        {
            if ( ! json.EndsWith(']'))
            {
                return false;
            }

            JsonNode.Parse(json);

            return true;
        }
        catch (Exception ex)
            when (ex is ArgumentException)
        {
            return false;
        }
    }

}
