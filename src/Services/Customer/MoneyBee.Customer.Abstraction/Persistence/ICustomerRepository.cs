namespace MoneyBee.Customer.Abstraction.Persistence
{
    public interface ICustomerRepository
    {
        Task AddAsync(Domain.Entities.Customer customer, CancellationToken cancellationToken);

        Task<Domain.Entities.Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Domain.Entities.Customer> GetByIdentityNumberAsync(string identityNumber, CancellationToken cancellationToken);

        Task<List<Domain.Entities.Customer>> SearchAsync(string query, CancellationToken cancellationToken);

        Task DeleteAsync(Domain.Entities.Customer customer, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}