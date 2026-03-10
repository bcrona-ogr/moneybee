using Microsoft.EntityFrameworkCore;
using MoneyBee.Auth.Abstraction.Persistence;
using MoneyBee.Auth.Domain.Entities;
using MoneyBee.Auth.Repository.Persistence.DbContexts;

namespace MoneyBee.Auth.Repository.Persistence.Repositories;

public  class EmployeeRepository : IEmployeeRepository
{
    private readonly AuthDbContext _dbContext;

    public EmployeeRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Employee> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.Employees.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}