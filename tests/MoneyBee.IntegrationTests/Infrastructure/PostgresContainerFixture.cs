using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Xunit;

namespace MoneyBee.IntegrationTests.Infrastructure
{
    public  class PostgresContainerFixture : IAsyncLifetime
    {
        private const string DbUser = "postgres";
        private const string DbPassword = "postgres";
        private const string MasterDatabase = "postgres";

        private readonly IContainer _container;

        public string Host => "127.0.0.1";
        public int Port => _container.GetMappedPublicPort(5432);

        public string AuthDatabaseName => "moneybee_auth_test";
        public string CustomerDatabaseName => "moneybee_customer_test";
        public string TransferDatabaseName => "moneybee_transfer_test";

        public string AuthConnectionString => BuildConnectionString(AuthDatabaseName);
        public string CustomerConnectionString => BuildConnectionString(CustomerDatabaseName);
        public string TransferConnectionString => BuildConnectionString(TransferDatabaseName);

        public PostgresContainerFixture()
        {
            _container = new ContainerBuilder()
                .WithImage("postgres:16")
                .WithPortBinding(5432, true)
                .WithEnvironment("POSTGRES_USER", DbUser)
                .WithEnvironment("POSTGRES_PASSWORD", DbPassword)
                .WithEnvironment("POSTGRES_DB", MasterDatabase)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            await WaitUntilDatabaseReadyAsync();

            await CreateDatabaseIfNotExistsAsync(AuthDatabaseName);
            await CreateDatabaseIfNotExistsAsync(CustomerDatabaseName);
            await CreateDatabaseIfNotExistsAsync(TransferDatabaseName);
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        private string BuildConnectionString(string databaseName)
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = Host,
                Port = Port,
                Username = DbUser,
                Password = DbPassword,
                Database = databaseName,
                Pooling = true
            }.ConnectionString;
        }

        private async Task WaitUntilDatabaseReadyAsync(int retryCount = 20)
        {
            var masterConnectionString = new NpgsqlConnectionStringBuilder
            {
                Host = Host,
                Port = Port,
                Username = DbUser,
                Password = DbPassword,
                Database = MasterDatabase,
                Pooling = true
            }.ConnectionString;

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    await using var connection = new NpgsqlConnection(masterConnectionString);
                    await connection.OpenAsync();

                    await using var command = new NpgsqlCommand("SELECT 1;", connection);
                    await command.ExecuteScalarAsync();

                    return;
                }
                catch
                {
                    if (i == retryCount - 1)
                        throw;

                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync(string databaseName)
        {
            var masterConnectionString = new NpgsqlConnectionStringBuilder
            {
                Host = Host,
                Port = Port,
                Username = DbUser,
                Password = DbPassword,
                Database = MasterDatabase,
                Pooling = true
            }.ConnectionString;

            await using var connection = new NpgsqlConnection(masterConnectionString);
            await connection.OpenAsync();

            await using var checkCommand = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @databaseName;",
                connection);

            checkCommand.Parameters.AddWithValue("databaseName", databaseName);

            var exists = await checkCommand.ExecuteScalarAsync();
            if (exists != null)
                return;

            await using var createCommand = new NpgsqlCommand(
                $"CREATE DATABASE \"{databaseName}\";",
                connection);

            await createCommand.ExecuteNonQueryAsync();
        }
    }
}