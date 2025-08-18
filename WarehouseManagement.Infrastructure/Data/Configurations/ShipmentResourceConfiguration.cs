using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ShipmentResourceConfiguration : IEntityTypeConfiguration<ShipmentResource>
{
    public void Configure(EntityTypeBuilder<ShipmentResource> builder)
    {
        builder.ToTable("ShipmentResources");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Quantity)
            .HasPrecision(18, 3);
            
        builder.HasOne(s => s.ShipmentDocument)
            .WithMany(d => d.ShipmentResources)
            .HasForeignKey(s => s.ShipmentDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(s => s.Resource)
            .WithMany(r => r.ShipmentResources)
            .HasForeignKey(s => s.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(s => s.UnitOfMeasurement)
            .WithMany(u => u.ShipmentResources)
            .HasForeignKey(s => s.UnitOfMeasurementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}