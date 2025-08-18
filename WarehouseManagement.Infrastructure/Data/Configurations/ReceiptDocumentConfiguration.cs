using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ReceiptDocumentConfiguration : IEntityTypeConfiguration<ReceiptDocument>
{
    public void Configure(EntityTypeBuilder<ReceiptDocument> builder)
    {
        builder.ToTable("ReceiptDocuments");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Number)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(r => r.Number)
            .IsUnique();
            
        builder.Property(r => r.Date)
            .IsRequired();
    }
}