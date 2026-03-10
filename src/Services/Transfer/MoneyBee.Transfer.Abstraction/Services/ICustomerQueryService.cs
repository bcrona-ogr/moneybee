namespace MoneyBee.Transfer.Abstraction.Services
{
    public interface ICustomerQueryService
    {
        Task<CustomerSummary> GetByIdAsync(Guid customerId, CancellationToken cancellationToken);
    }
}