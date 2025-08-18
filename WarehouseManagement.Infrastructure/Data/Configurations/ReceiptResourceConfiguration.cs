using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ReceiptResourceConfiguration : IEntityTypeConfiguration<ReceiptResource>
{
    public void Configure(EntityTypeBuilder<ReceiptResource> builder)
    {
        builder.ToTable("ReceiptResources");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Quantity)
            .HasPrecision(18, 3);
            
        builder.HasOne(r => r.ReceiptDocument)
            .WithMany(d => d.ReceiptResources)
            .HasForeignKey(r => r.ReceiptDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(r => r.Resource)
            .WithMany(res => res.ReceiptResources)
            .HasForeignKey(r => r.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(r => r.UnitOfMeasurement)
            .WithMany(u => u.ReceiptResources)
            .HasForeignKey(r => r.UnitOfMeasurementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}