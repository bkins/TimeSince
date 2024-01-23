
namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService // Secrets Service Methods
{
    public string GetSecretValue(SecretCollections collection
                               , SecretKeys        key)
    {
        return SecretsService.Instance.GetSecretValue(collection
                                                    , key);
    }
}
