using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyBee.Auth.Contracts.Login.Request;
using MoneyBee.Auth.Contracts.Login.Response;
using MoneyBee.Shared.API.Response;
using MoneyBee.IntegrationTests.Infrastructure;
using Xunit.Abstractions;

namespace MoneyBee.IntegrationTests.Auth
{
    [Collection("postgres")]
    public class AuthApiTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly AuthApiFactory _factory;

        public AuthApiTests(PostgresContainerFixture postgres, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = new AuthApiFactory(postgres.AuthConnectionString);
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }


        [Fact]
        public async Task Login_Should_Return200AndToken_When_CredentialsValid()
        {
            var request = new LoginHttpRequest
            {
                Username = "admin",
                Password = "123456"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);
            var raw = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<LoginHttpResponse>();

            body.Should().NotBeNull();
            body.AccessToken.Should().NotBeNullOrWhiteSpace();
            body.TokenType.Should().Be("Bearer");
            body.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_Should_Return400_When_PasswordInvalid()
        {
            var request = new LoginHttpRequest
            {
                Username = "admin",
                Password = "wrong-password"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);
            var raw = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            body.Message.Should().Be("Invalid username or password.");
        }

        [Fact]
        public async Task Login_Should_Return400_When_EmployeePassive()
        {
            var request = new LoginHttpRequest
            {
                Username = "passive-user",
                Password = "passive-password"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);
            var raw = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Employee is passive.");
        }

        [Fact]
        public async Task Login_Should_Return422_When_RequestInvalid()
        {
            var request = new LoginHttpRequest
            {
                Username = "",
                Password = ""
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);
            var raw = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            response.StatusCode.Should().Be((HttpStatusCode) 422);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.StatusCode.Should().Be(422);
            body.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Me_Should_Return401_When_TokenMissing()
        {
            var response = await _client.GetAsync("/api/auth/me");
            var raw = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Me_Should_Return200_When_TokenValid()
        {
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginHttpRequest
            {
                Username = "admin",
                Password = "123456"
            });
            var raw = await loginResponse.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw);
            Console.WriteLine(raw);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginHttpResponse>();

            loginBody.Should().NotBeNull();
            loginBody.AccessToken.Should().NotBeNullOrWhiteSpace();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

            var response = await _client.GetAsync("/api/auth/me");
            var raw1 = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(raw1);
            Console.WriteLine(raw1);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();

            body.Should().Contain("admin");
        }
    }
}