using Microsoft.EntityFrameworkCore;

namespace MoneyBee.Customer.Repository.Persistence.DbContexts
{
    public  class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Customer> Customers
        {
            get => Set<Domain.Entities.Customer>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}