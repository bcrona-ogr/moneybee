using Xunit;

namespace MoneyBee.IntegrationTests.Infrastructure
{
    [CollectionDefinition("postgres")]
    public  class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
    {
    }
}