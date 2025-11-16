using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Customers.Entities;
using OrderManagement.Domain.Orders.Entities;

namespace OrderManagement.Infrastructure.Persistence.Orders;

/// <summary>
/// Order Configuration
/// </summary>
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders").HasKey(o => o.Id);
        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.Type).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.DeliveryTimeNeeded);

        // Computed in domain, not persisted explicitly as a column
        builder.Ignore(o => o.Total);

        builder.Property<byte[]>("RowVersion").IsRowVersion();

        // FK: Orders → Customers (CustomerId)
        builder.HasOne<Customer>().WithMany().HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);

        // Owned value object: DeliveryAddress
        builder.OwnsOne(o => o.DeliveryAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).HasColumnName("DeliveryStreet");
            address.Property(a => a.City).HasMaxLength(100).HasColumnName("DeliveryCity");
            address.Property(a => a.Zip).HasMaxLength(20).HasColumnName("DeliveryZip");
            address.Property(a => a.Line2).HasMaxLength(200).HasColumnName("DeliveryLine2");
            address.Property(a => a.Country).HasMaxLength(100).HasColumnName("DeliveryCountry");
        });

        // Owned value object: AssignmentInfo
        builder.OwnsOne(o => o.Assignment, assignment =>
        {
            assignment.Property(a => a.CourierId).HasColumnName("AssignmentCourierId");
            assignment.Property(a => a.AssignedAt).HasColumnName("AssignmentAssignedAt");
            assignment.Property(a => a.OutForDeliveryAt).HasColumnName("AssignmentOutForDeliveryAt");
            assignment.Property(a => a.DeliveredAt).HasColumnName("AssignmentDeliveredAt");
            assignment.Property(a => a.UnableToDeliverAt).HasColumnName("AssignmentUnableToDeliverAt");

            // FK: Assignment.CourierId → Staff.Id
            assignment.HasOne<Domain.Staff.Entities.Staff>().WithMany().HasForeignKey(a => a.CourierId).OnDelete(DeleteBehavior.Restrict);
            assignment.HasIndex(a => a.CourierId);
        });

        // Owned collection: OrderItems
        builder.OwnsMany(o => o.Items, items =>
        {
            items.ToTable("OrderItems");
            items.WithOwner().HasForeignKey("OrderId");
            items.HasKey("OrderId", "MenuItemId");

            items.Property(i => i.MenuItemId).IsRequired();
            items.Property(i => i.Name).IsRequired().HasMaxLength(200);
            items.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            items.Property(i => i.Quantity).IsRequired();
            items.Property(i => i.Notes).HasMaxLength(500);

            // Ignore computed Total on the value object (if present)
            items.Ignore(i => i.Total);
        });

        // Indexes for queries
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.Type);
        builder.HasIndex(o => o.CreatedAt);
    }
}