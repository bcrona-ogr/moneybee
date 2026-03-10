using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace MoneyBee.IntegrationTests.Gateway
{
    public  class GatewayApiTests : IAsyncLifetime
    {
        private FakeDownstreamServer _downstreamServer;
        private GatewayApiFactory _factory;
        private HttpClient _client;

        public async Task InitializeAsync()
        {
            _downstreamServer = new FakeDownstreamServer();
            await _downstreamServer.StartAsync();

            _factory = new GatewayApiFactory(_downstreamServer.BaseAddress);
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            _factory?.Dispose();

            if (_downstreamServer != null)
                await _downstreamServer.DisposeAsync();
        }

        [Fact]
        public async Task Health_Should_Return200()
        {
            var response = await _client.GetAsync("/gateway/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("MoneyBee.ApiGateway");
            body.Should().Contain("Healthy");
        }

        [Fact]
        public async Task AuthRoute_Should_Proxy_Login_Request()
        {
            var response = await _client.PostAsJsonAsync("/gateway/api/auth/login", new
            {
                username = "admin",
                password = "123456"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("fake-token");
            body.Should().Contain("Bearer");
        }

        [Fact]
        public async Task CustomerRoute_Should_Return401_When_TokenMissing()
        {
            var response = await _client.GetAsync($"/gateway/api/customers/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task TransferRoute_Should_Return401_When_TokenMissing()
        {
            var response = await _client.PostAsJsonAsync("/gateway/api/transfers", new
            {
                senderCustomerId = Guid.NewGuid(),
                receiverCustomerId = Guid.NewGuid(),
                amount = 100m
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CustomerRoute_Should_Forward_Request_And_Authorization_Header()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/gateway/api/customers/{Guid.NewGuid()}");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _factory.CreateToken());

            var response = await _client.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);
            raw.Should().Contain("/api/customers/");
            raw.Should().Contain("Bearer ");
            raw.Should().Contain("customer");
        }

        [Fact]
        public async Task CustomerPostRoute_Should_Forward_Request_And_Authorization_Header()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/gateway/api/customers")
            {
                Content = JsonContent.Create(new
                {
                    firstName = "Ali",
                    lastName = "Veli"
                })
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _factory.CreateToken());

            var response = await _client.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);
            raw.Should().Contain("/api/customers");
            raw.Should().Contain("Bearer ");
            raw.Should().Contain("customer");
        }

        [Fact]
        public async Task TransferRoute_Should_Forward_Request_And_Authorization_Header()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/gateway/api/transfers")
            {
                Content = JsonContent.Create(new
                {
                    senderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    receiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    amount = 100m
                })
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _factory.CreateToken());

            var response = await _client.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);
            raw.Should().Contain("/api/transfers");
            raw.Should().Contain("Bearer ");
            raw.Should().Contain("transfer");
        }
    }
}