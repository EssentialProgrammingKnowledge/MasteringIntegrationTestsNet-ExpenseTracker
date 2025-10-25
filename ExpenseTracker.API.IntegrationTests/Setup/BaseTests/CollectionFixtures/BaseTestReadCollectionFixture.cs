using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures
{
    [Collection(IntegrationTestsReadCollection.COLLECTION_NAME)]
    public abstract class BaseTestReadCollectionFixture : BaseTest
    {
        protected BaseTestReadCollectionFixture(WebAppFactoryFixture factory, ITestOutputHelper testOutputHelper)
            : base(factory, testOutputHelper)
        {
        }
    }

    [CollectionDefinition(COLLECTION_NAME)]
    public class IntegrationTestsReadCollection : ICollectionFixture<WebAppFactoryFixture>
    {
        public const string COLLECTION_NAME = "ExpenseTrackerReadCollectionFixture";
    }
}
