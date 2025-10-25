using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures
{
    [Collection(IntegrationTestsWriteCollection.COLLECTION_NAME)]
    public abstract class BaseTestWriteCollectionFixture : BaseTest
    {
        public BaseTestWriteCollectionFixture(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        { }
    }

    [CollectionDefinition(COLLECTION_NAME)]
    public class IntegrationTestsWriteCollection : ICollectionFixture<WebAppFactoryFixture>
    {
        public const string COLLECTION_NAME = "ExpenseTrackerWriteCollectionFixture";
    }
}
