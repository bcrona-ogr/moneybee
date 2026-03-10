using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoneyBee.Customer.Repository.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Domain.Entities.Customer>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Customer> builder)
        {
            builder.ToTable("customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
            builder.Property(x => x.IdentityNumber).HasMaxLength(50).IsRequired();

            builder.Property(x => x.DateOfBirth).HasColumnType("date").IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.UpdatedAtUtc).IsRequired();
            builder.HasQueryFilter(x => x.Active);
            builder.HasIndex(x => x.IdentityNumber).IsUnique();
        }
    }
}