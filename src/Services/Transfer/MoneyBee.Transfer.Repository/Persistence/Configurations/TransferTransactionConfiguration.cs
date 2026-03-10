using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Repository.Persistence.Configurations
{
    public  class TransferTransactionConfiguration : IEntityTypeConfiguration<TransferTransaction>
    {
        public void Configure(EntityTypeBuilder<TransferTransaction> builder)
        {
            builder.ToTable("transfers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SenderCustomerId).IsRequired();
            builder.Property(x => x.ReceiverCustomerId).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(x => x.Fee).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(x => x.Currency).IsRequired();
            builder.Property(x => x.TransactionCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedByEmployeeId).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasQueryFilter(x => x.Active);
            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.HasIndex(x => x.TransactionCode).IsUnique();
            builder.HasIndex(x => new
            {
                x.SenderCustomerId,
                x.CreatedAtUtc
            });
        }
    }
}