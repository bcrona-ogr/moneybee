using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyBee.Shared.API.Response;
using MoneyBee.Customer.Contracts.Requests.Create;
using MoneyBee.Customer.Contracts.Requests.Update;
using MoneyBee.Customer.Contracts.Responses;
using MoneyBee.Customer.Contracts.Responses.Search;
using MoneyBee.IntegrationTests.Infrastructure;

namespace MoneyBee.IntegrationTests.Customer
{
    [Collection("postgres")]
    public class CustomerApiTests : IDisposable
    {
        private readonly CustomerApiFactory _factory;
        private readonly HttpClient _client;

        public CustomerApiTests(PostgresContainerFixture postgres)
        {
            _factory = new CustomerApiFactory(postgres.CustomerConnectionString);
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task Create_Should_Return401_When_TokenMissing()
        {
            var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerHttpRequest
            {
                FirstName = "Mehmet",
                LastName = "Yilmaz",
                PhoneNumber = "5553334455",
                Address = "Izmir",
                DateOfBirth = new DateTime(1993, 3, 3),
                IdentityNumber = GenerateIdentityNumber()
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_Should_Return200_When_RequestValid()
        {
            AuthorizeClient();

            var identityNumber = GenerateIdentityNumber();

            var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerHttpRequest
            {
                FirstName = "Mehmet",
                LastName = "Yilmaz",
                PhoneNumber = "5553334455",
                Address = "Izmir",
                DateOfBirth = new DateTime(1993, 3, 3),
                IdentityNumber = identityNumber
            });

            var raw = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);

            var body = await response.Content.ReadFromJsonAsync<CustomerHttpResponse>();

            body.Should().NotBeNull();
            body.Id.Should().NotBe(Guid.Empty);
            body.FirstName.Should().Be("Mehmet");
            body.LastName.Should().Be("Yilmaz");
            body.IdentityNumber.Should().Be(identityNumber);
        }

        [Fact]
        public async Task Create_Should_Return422_When_RequestInvalid()
        {
            AuthorizeClient();

            var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerHttpRequest
            {
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Address = "",
                DateOfBirth = DateTime.UtcNow.AddDays(1),
                IdentityNumber = ""
            });

            response.StatusCode.Should().Be((HttpStatusCode)422);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.StatusCode.Should().Be(422);
        }

        [Fact]
        public async Task Create_Should_Return400_When_IdentityNumberAlreadyExists()
        {
            var identityNumber = GenerateIdentityNumber();

            await CreateCustomerAsync(
                firstName: "Ali",
                lastName: "Veli",
                identityNumber: identityNumber);

            AuthorizeClient();

            var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerHttpRequest
            {
                FirstName = "Other",
                LastName = "User",
                PhoneNumber = "5559999999",
                Address = "Bursa",
                DateOfBirth = new DateTime(1991, 1, 1),
                IdentityNumber = identityNumber
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Customer with the same identity number already exists.");
        }

        [Fact]
        public async Task GetById_Should_Return200_When_CustomerExists()
        {
            var created = await CreateCustomerAsync(firstName: "Ali");

            AuthorizeClient();

            var response = await _client.GetAsync($"/api/customers/{created.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<CustomerHttpResponse>();

            body.Should().NotBeNull();
            body.Id.Should().Be(created.Id);
            body.FirstName.Should().Be("Ali");
        }

        [Fact]
        public async Task GetById_Should_Return404_When_CustomerNotFound()
        {
            AuthorizeClient();

            var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Customer not found.");
        }

        [Fact]
        public async Task Search_Should_Return200_When_QueryMatches()
        {
            await CreateCustomerAsync(firstName: "AliSearchTarget");

            AuthorizeClient();

            var response = await _client.GetAsync("/api/customers?query=AliSearchTarget");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<SearchCustomersHttpResponse>();

            body.Should().NotBeNull();
            body.Items.Should().NotBeNull();
            body.Items.Should().Contain(x => x.FirstName == "AliSearchTarget");
        }

        [Fact]
        public async Task Update_Should_Return200_When_CustomerExists()
        {
            var created = await CreateCustomerAsync(
                firstName: "Ali",
                lastName: "Veli");

            AuthorizeClient();

            var response = await _client.PutAsJsonAsync($"/api/customers/{created.Id}", new UpdateCustomerHttpRequest
            {
                FirstName = "AliUpdated",
                LastName = "VeliUpdated",
                PhoneNumber = "5550001111",
                Address = "Updated Address",
                DateOfBirth = new DateTime(1989, 9, 9)
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<CustomerHttpResponse>();

            body.Should().NotBeNull();
            body.FirstName.Should().Be("AliUpdated");
            body.LastName.Should().Be("VeliUpdated");
            body.PhoneNumber.Should().Be("5550001111");
            body.Address.Should().Be("Updated Address");
            body.DateOfBirth.Should().Be(new DateTime(1989, 9, 9));
        }

        [Fact]
        public async Task Delete_Should_Return204_When_CustomerExists()
        {
            var created = await CreateCustomerAsync(firstName: "DeleteMe");

            AuthorizeClient();

            var response = await _client.DeleteAsync($"/api/customers/{created.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Should_Return404_When_CustomerNotFound()
        {
            AuthorizeClient();

            var response = await _client.DeleteAsync($"/api/customers/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Customer not found.");
        }

        private async Task<CustomerHttpResponse> CreateCustomerAsync(
            string firstName = "Ali",
            string lastName = "Veli",
            string phoneNumber = "5553334455",
            string address = "Istanbul",
            DateTime? dateOfBirth = null,
            string identityNumber = null)
        {
            AuthorizeClient();

            var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerHttpRequest
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address,
                DateOfBirth = dateOfBirth ?? new DateTime(1990, 1, 1),
                IdentityNumber = identityNumber ?? GenerateIdentityNumber()
            });

            var raw = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, raw);

            var body = await response.Content.ReadFromJsonAsync<CustomerHttpResponse>();
            body.Should().NotBeNull();

            return body;
        }

        private static string GenerateIdentityNumber()
        {
            return $"{Random.Shared.NextInt64(10000000000, 99999999999)}";
        }

        private void AuthorizeClient()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestJwtTokenFactory.CreateToken());
        }
    }
}