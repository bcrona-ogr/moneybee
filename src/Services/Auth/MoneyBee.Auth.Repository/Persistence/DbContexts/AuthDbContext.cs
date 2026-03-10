using Microsoft.EntityFrameworkCore;
using MoneyBee.Auth.Domain.Entities;

namespace MoneyBee.Auth.Repository.Persistence.DbContexts;

public  class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees
    {
        get => Set<Employee>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}