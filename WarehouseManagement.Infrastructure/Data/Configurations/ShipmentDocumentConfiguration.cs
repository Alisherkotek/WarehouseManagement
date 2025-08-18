using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ShipmentDocumentConfiguration : IEntityTypeConfiguration<ShipmentDocument>
{
    public void Configure(EntityTypeBuilder<ShipmentDocument> builder)
    {
        builder.ToTable("ShipmentDocuments");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Number)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(s => s.Number)
            .IsUnique();
            
        builder.Property(s => s.Date)
            .IsRequired();
            
        builder.Property(s => s.Status)
            .IsRequired();
            
        builder.HasOne(s => s.Client)
            .WithMany(c => c.ShipmentDocuments)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}