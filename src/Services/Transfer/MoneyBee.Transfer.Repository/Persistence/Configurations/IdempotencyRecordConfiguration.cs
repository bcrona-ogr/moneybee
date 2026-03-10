using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Repository.Persistence.Configurations
{
    public  class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("idempotency_records");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdempotencyKey).HasMaxLength(200).IsRequired();
            builder.Property(x => x.OperationName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.ActorId).IsRequired();
            builder.Property(x => x.RequestHash).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ResponsePayload).IsRequired(false);
            builder.Property(x => x.ResponseStatusCode).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.ExpiresAtUtc).IsRequired();
            builder.HasQueryFilter(x => x.Active);
            builder.HasIndex(x => new
            {
                x.IdempotencyKey,
                x.OperationName,
                x.ActorId
            }).IsUnique();
        }
    }
}