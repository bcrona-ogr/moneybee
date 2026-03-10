using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyBee.Auth.Domain.Entities;

namespace MoneyBee.Auth.Repository.Persistence.Configurations;

public  class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.PasswordSalt)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();
        builder.HasQueryFilter(x => x.Active);

        builder.HasIndex(x => x.Username)
            .IsUnique();
    }
}