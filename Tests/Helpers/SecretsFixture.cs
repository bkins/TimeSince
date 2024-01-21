using TimeSince.Avails;

namespace Tests.Helpers;

public class SecretsFixture : IDisposable
{
    public Secrets SecretsInstance { get; }

    public SecretsFixture()
    {
        SecretsInstance = new Secrets(new Secrets.FileJsonContentProvider().GetJsonContent);
    }

    public void Dispose()
    {
        // Dispose logic, if needed
    }
}
