using TimeSince.Avails;

namespace Testing.Helpers;

public class SecretsFixture : IDisposable
{
    public Secrets SecretsInstance { get; }

    public SecretsFixture()
    {
        SecretsInstance = new Secrets("TimeSince.secrets.keys.json");
    }

    public void Dispose()
    {
        // Dispose logic, if needed
    }
}
