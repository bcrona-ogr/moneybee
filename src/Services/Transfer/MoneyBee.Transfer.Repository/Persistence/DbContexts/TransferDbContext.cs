using Microsoft.EntityFrameworkCore;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Repository.Persistence.DbContexts
{
    public  class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyBusinessException("Record changed by another operation. Please retry.");
            }
        }

        public DbSet<IdempotencyRecord> IdempotencyRecords
        {
            get => Set<IdempotencyRecord>();
        }

        public DbSet<TransferTransaction> Transfers
        {
            get => Set<TransferTransaction>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransferDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}