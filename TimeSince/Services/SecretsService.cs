using TimeSince.Avails;
using TimeSince.Avails.SecretsEnums;

namespace TimeSince.Services;

public class SecretsService
{
    // ReSharper disable once InconsistentNaming
    private static readonly SecretsService? _instance = null;
    public  static          SecretsService  Instance => _instance ?? new SecretsService();

    private Secrets Secrets { get; } = new("TimeSince.secrets.keys.json");

    public string? GetSecretValue(SecretCollections collection
                                , SecretKeys        key)
    {
        return Secrets.GetSecretValue(collection
                                    , key);
    }
}
