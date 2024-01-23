using TimeSince.Avails;

namespace TimeSince.Services;

public class SecretsService
{
    private static SecretsService? _instance;
    public  static SecretsService  Instance => _instance ?? new SecretsService();

    private Secrets Secrets { get; } = new("TimeSince.secrets.keys.json");

    public string GetSecretValue(SecretCollections collection
                               , SecretKeys        key)
    {
        return Secrets.GetSecretValue(collection
                                    , key);
    }
}
