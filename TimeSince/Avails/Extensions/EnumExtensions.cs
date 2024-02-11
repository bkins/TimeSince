using System.ComponentModel;
using System.Reflection;

namespace TimeSince.Avails.Extensions;

/// <summary>
/// Provides extension methods for Enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description attribute value of an Enum value.
    /// </summary>
    /// <param name="value">The Enum value.</param>
    /// <returns>
    /// The description attribute value if present; otherwise, the Enum value's string representation.
    /// </returns>
    public static string GetDescription(this Enum value)
    {
        var field     = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null
                    ? value.ToString()
                    : attribute.Description;
    }
}
