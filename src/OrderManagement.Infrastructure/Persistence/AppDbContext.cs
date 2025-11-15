using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Customers.Entities;
using OrderManagement.Domain.Menu.Entities;
using OrderManagement.Domain.Orders.Entities;

namespace OrderManagement.Infrastructure.Persistence;

/// <summary>
/// App database context
/// </summary>
/// <seealso cref="DbContext" />
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the customers.
    /// </summary>
    public DbSet<Customer> Customers { get; set; } = default!;

    /// <summary>
    /// Gets or sets the menu items.
    /// </summary>
    public DbSet<MenuItem> MenuItems { get; set; } = default!;

    /// <summary>
    /// Gets or sets the orders.
    /// </summary>
    public DbSet<Order> Orders { get; set; } = default!;

    /// <summary>
    /// Gets or sets the staff.
    /// </summary>
    public DbSet<Domain.Staff.Entities.Staff> Staff { get; set; } = default!;

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
    /// define extension methods on this object that allow you to configure aspects of the model that are specific
    /// to a given database.</param>
    /// <remarks>
    /// <para>
    /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    /// then this method will not be run. However, it will still run when creating a compiled model.
    /// </para>
    /// <para>
    /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    /// examples.
    /// </para>
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
