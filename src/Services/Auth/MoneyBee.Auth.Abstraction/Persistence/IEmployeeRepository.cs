using MoneyBee.Auth.Domain.Entities;

namespace MoneyBee.Auth.Abstraction.Persistence;

public interface IEmployeeRepository
{
    Task<Employee> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task AddAsync(Employee employee, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}