using System.ComponentModel;

namespace TimeSince.Avails;

public static class Utilities
{
    public static T? GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == description)
                {
                    return (T)field.GetValue(null)!;
                }
            }
            else if (field.Name == description)
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new ArgumentException($"No {typeof(T)} with Description or Name as '{description}' found");
    }

    public static async Task<T> ForceAsync<T>(Func<Task<T>> func)
    {
        await Task.Yield();
        return await func();
    }
}
