using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TimeSince.Avails
{
    /// <summary>
    /// This class is to handle secret values.
    /// To use, set the 'resourceName' to the JSON file that holds the secrets
    /// when newing up this class (e.g. TimeSince.secrets.keys.json).
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
    public class Secrets(string resourceName)
    {
        private const    string IdName       = "keyName";
        private const    string IdValue      = "keyValue";
        private readonly string _jsonContent = LoadJsonContent(resourceName);

        public string GetSecretValue(SecretCollections collection, SecretKeys key)
        {
            var jsonObj = JObject.Parse(_jsonContent);
            var id      = jsonObj[collection.ToString().ToLower()]?.FirstOrDefault(token => (string)token[IdName] == key.ToString());

            return id != null ? (string)id[IdValue] : null;
        }

        private static string LoadJsonContent(string resourceName)
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
