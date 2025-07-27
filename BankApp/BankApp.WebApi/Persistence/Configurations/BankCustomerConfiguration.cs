using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankApp.WebApi.Entities;

namespace BankApp.WebApi.Persistence.Configurations;

public class BankCustomerConfiguration : IEntityTypeConfiguration<BankCustomer>
{
    public void Configure(EntityTypeBuilder<BankCustomer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Document).IsRequired().HasMaxLength(14);
        builder.Property(x => x.BirthDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}