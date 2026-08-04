using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.CustomerName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Branch).IsRequired().HasMaxLength(50);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasMany(s => s.Products)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId);

        builder
            .HasIndex(u => u.Number)
            .IsUnique();
    }
}
