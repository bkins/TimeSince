using System.ComponentModel;
using System.Reflection;

namespace TimeSince.Avails.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field     = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null
                    ? value.ToString()
                    : attribute.Description;
    }
}
