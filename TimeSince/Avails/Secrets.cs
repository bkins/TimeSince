using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TimeSince.Avails;

/// <summary>
/// This class is to handle secret values.
/// To use, set the 'resourceName' in the 'GetSecretValue' method to the JSON file that holds the secrets.
/// See below of an example of the contents of the secrets.keys.json file:
/// </summary>
/* Example of the contents of the secrets.keys.json file:
// {
//     "admob":
//     [
//     {
//         "keyName": "MainPageBanner"
//       , "keyValue":"ca-app-pub-12341234123412340/1234512345"
//     }
//   , {
//         "keyName": "AppId"
//       , "keyValue": "ca-app-pub-12341234123412340~1234512345"
//     }
//   , {
//         "keyName": "MainPageNewEventInterstitial"
//       , "keyValue": "ca-app-pub-12341234123412340/1234512345"
//     }
//   , {
//         "keyName": "MainPageRewarded"
//       , "keyValue": "ca-app-pub-12341234123412340/1234512345"
//     }
//     ]
// }
*/
public class Secrets
{
    private const    string               IdName  = "keyName";
    private const    string               IdValue = "keyValue";
    private readonly Func<string, string> _jsonContentProvider;

    public Secrets(Func<string, string> jsonContentProvider)
    {
        _jsonContentProvider = jsonContentProvider;
    }

    public string GetSecretValue(SecretCollections collection, SecretKeys key)
    {
        const string resourceName = "TimeSince.secrets.keys.json";

        var jsonContent = _jsonContentProvider(resourceName);

        var jsonObj = JObject.Parse(jsonContent);
        var id = jsonObj[collection.ToString()
                                   .ToLower()]?.FirstOrDefault(token => (string) token[IdName] == key.ToString());

        return id != null ? (string) id[IdValue] : null;
    }

    // public string GetSecretValue(SecretCollections collection, SecretKeys key)
    // {
    //     const string resourceName = "TimeSince.secrets.keys.json";
    //
    //     var jsonContent = GetSecretsJsonContent(resourceName);
    //
    //     var jsonObj = JObject.Parse(jsonContent);
    //     var id      = jsonObj[collection.ToString()
    //                                     .ToLower()]?.FirstOrDefault(token => (string)token[IdName] == key.ToString());
    //
    //     if (id != null)
    //     {
    //         return (string)id[IdValue];
    //     }
    //
    //     return null;
    // }

    public string GetSecretsJsonContent(string resourceName)
    {
        var          assembly       = Assembly.GetExecutingAssembly();
        using var    resourceStream = assembly.GetManifestResourceStream(resourceName);

        if (resourceStream == null) return string.Empty;

        using var reader = new StreamReader(resourceStream);
        var jsonContent = reader.ReadToEnd();

        return jsonContent;
    }
    public class FileJsonContentProvider
    {
        public string GetJsonContent(string resourceName)
        {
            var       assembly       = Assembly.GetExecutingAssembly();
            using var resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream == null) return string.Empty;

            using var reader = new StreamReader(resourceStream);
            return reader.ReadToEnd();
        }
    }
}

public enum SecretKeys
{
    MainPageBanner
  , AppId
  , MainPageNewEventInterstitial
  , MainPageRewarded
  , AppSecretKey
  , SyncFusionLicense
  , DoorKey
  , DoorKeyReadOnly
}

public enum SecretCollections
{
    Admob
  , AppCenter
  , SyncFusion
  , AppControl
}
