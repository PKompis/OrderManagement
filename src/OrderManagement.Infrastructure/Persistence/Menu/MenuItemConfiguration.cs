using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Infrastructure.Persistence.Menu;

/// <summary>
/// Menu Item Configuration
/// </summary>
public sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    /// <summary>
    /// Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems").HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.IsAvailable).IsRequired();

        builder.Property<byte[]>("RowVersion").IsRowVersion();
    }
}