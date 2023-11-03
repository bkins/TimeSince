using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TimeSince.Avails;

public static class Utilities
{
    private const string IdName = "keyName";
    private const string IdValue   = "keyValue";

    public static T GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            else if (field.Name == description)
            {
                return (T)field.GetValue(null);
            }
        }

        throw new ArgumentException($"No {typeof(T)} with Description or Name as '{description}' found");
    }

    public static string GetSecretValue(SecretCollections collection, SecretKeys key)
    {
        var jsonContent = string.Empty;

        var          assembly     = Assembly.GetExecutingAssembly();
        const string resourceName = "TimeSince.secrets.keys.json";
        using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream != null)
            {
                using var reader = new StreamReader(resourceStream);
                jsonContent = reader.ReadToEnd();
            }
        }

        var jsonObj     = JObject.Parse(jsonContent);
        var id          = jsonObj[collection.ToString().ToLower()]?.FirstOrDefault(token => (string)token[IdName] == key.ToString());

        if (id != null)
        {
            return (string)id[IdValue];
        }

        return null;
    }

    public static async Task<T> ForceAsync<T>(Func<Task<T>> func)
    {
        await Task.Yield();
        return await func();
    }
}

public enum SecretKeys
{
    MainPageBanner
  , AppId
  , MainPageNewEventInterstitial
  , MainPageRewarded
}

public enum SecretCollections
{
    Admob
}
