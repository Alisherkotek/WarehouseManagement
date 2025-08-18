using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasFilter("[IsArchived] = 0");
            
        builder.Property(c => c.IsArchived)
            .HasDefaultValue(false);
    }
}