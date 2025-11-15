using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderManagement.Infrastructure.Persistence.Staff;

/// <summary>
/// Staff Configuration
/// </summary>
public sealed class StaffConfiguration : IEntityTypeConfiguration<Domain.Staff.Entities.Staff>
{
    /// <summary>
    /// Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Domain.Staff.Entities.Staff> builder)
    {
        builder.ToTable("Staff").HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.Property<byte[]>("RowVersion").IsRowVersion();

        builder.HasIndex(x => new { x.IsActive, x.Role });
    }
}