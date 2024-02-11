using System.Reflection;
using Testing.Helpers;
using TimeSince.Avails;
using TimeSince.Avails.SecretsEnums;

namespace Testing.Avails
{
    [CollectionDefinition("SecretsCollection")]
    public class SecretsCollection : ICollectionFixture<SecretsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("SecretsCollection")]
    public class SecretsTests
    {
        private readonly Secrets _secrets;

        public SecretsTests(SecretsFixture fixture)
        {
            _secrets = fixture.SecretsInstance;
        }

        [Fact]
        public void GetSecretValue_ReturnsCorrectValue()
        {
            // Arrange
            const string            expectedValue = "ca-app-pub-1259969651432104/6638245364";
            const SecretCollections collection    = SecretCollections.Admob;
            const SecretKeys        key           = SecretKeys.MainPageBanner;
            const string testJsonContent = "{"
                                         + "  \"admob\": ["
                                         + "    {"
                                         + "      \"keyName\": \"MainPageBanner\","
                                         + "      \"keyValue\": \"ca-app-pub-1259969651432104/6638245364\""
                                         + "    }"
                                         + "  ]"
                                         + "}";

            var secrets = new Secrets("TimeSince.secrets.keys.json");

            var type  = typeof(Secrets);
            var field = type.GetField("jsonContentProvider"
                                    , BindingFlags.Instance
                                    | BindingFlags.NonPublic);

            field?.SetValue(secrets, (Func<string, string>)(_ => testJsonContent));

            // Act
            var actualValue = secrets.GetSecretValue(collection, key);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetSecretValue_InvalidCollection_ReturnsNull()
        {
            // Arrange
            const SecretKeys key                   = SecretKeys.MainPageBanner;
            const string     invalidCollectionName = "InvalidCollection";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                if (Enum.TryParse(invalidCollectionName, true, out SecretCollections invalidCollection))
                {
                    _secrets.GetSecretValue(invalidCollection, key);
                }
                else
                {
                    throw new ArgumentException($"Invalid collection name: {invalidCollectionName}");
                }
            });

            Assert.Equal($"Invalid collection name: {invalidCollectionName}", exception.Message);
        }


        [Fact]
        public void GetSecretValue_InvalidKey_ReturnsNull()
        {
            // Arrange
            const SecretCollections collection     = SecretCollections.Admob;
            const string            invalidKeyName = "InvalidKey";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                if (Enum.IsDefined(typeof(SecretKeys), invalidKeyName))
                {
                    var invalidKey = (SecretKeys)Enum.Parse(typeof(SecretKeys), invalidKeyName);

                    _secrets.GetSecretValue(collection, invalidKey);
                }
                else
                {
                    throw new ArgumentException($"Invalid key name: {invalidKeyName}");
                }
            });

            Assert.Equal($"Invalid key name: {invalidKeyName}", exception.Message);
        }
    }

}
