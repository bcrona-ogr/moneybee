using Microsoft.EntityFrameworkCore;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.Repository.Persistence.DbContexts;

namespace MoneyBee.Customer.Repository.Persistence.Repositories
{
    public  class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _dbContext;

        public CustomerRepository(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Domain.Entities.Customer customer, CancellationToken cancellationToken)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task<Domain.Entities.Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Domain.Entities.Customer> GetByIdentityNumberAsync(string identityNumber, CancellationToken cancellationToken)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(x => x.IdentityNumber == identityNumber, cancellationToken);
        }

        public async Task<List<Domain.Entities.Customer>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            query = query?.Trim() ?? string.Empty;

            return await _dbContext.Customers
                .Where(x =>
                    x.FirstName.Contains(query) ||
                    x.LastName.Contains(query) ||
                    x.IdentityNumber.Contains(query) ||
                    x.PhoneNumber.Contains(query))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        public Task DeleteAsync(Domain.Entities.Customer customer, CancellationToken cancellationToken)
        {
            customer.SoftDelete(DateTime.UtcNow);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}