using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures
{
    [Collection(IntegrationTestsAllCollection.COLLECTION_NAME)]
    public class BaseTestCollectionFixture : BaseTest
    {
        public BaseTestCollectionFixture(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        { }
    }

    [CollectionDefinition(COLLECTION_NAME)]
    public class IntegrationTestsAllCollection : ICollectionFixture<WebAppFactoryFixture>
    {
        public const string COLLECTION_NAME = "ExpenseTrackerAllCollectionFixture";
    }
}
