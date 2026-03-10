using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Services;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public class CustomerQueryService : ICustomerQueryService
    {
        private readonly HttpClient _httpClient;
        private IHttpContextAccessor _httpContextAccessor;

        public CustomerQueryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CustomerSummary> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customers/{customerId}");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.TryAddWithoutValidation("Authorization", token);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<InternalCustomerSummaryResponse>(cancellationToken: cancellationToken);

            if (body == null)
                throw new BusinessException("Customer service returned an empty response.");

            return new CustomerSummary
            {
                Id = body.Id,
                FirstName = body.FirstName,
                LastName = body.LastName,
                IdentityNumber = body.IdentityNumber
            };
        }
    }
}