using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Infrastructure.Persistence.Menu;

/// <summary>
/// Menu Item Repository
/// </summary>
/// <seealso cref="IMenuItemRepository" />
public sealed class MenuItemRepository(AppDbContext dbContext) : IMenuItemRepository
{
    /// <summary>
    /// Gets the menu by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return default;

        return await dbContext.MenuItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Adds to the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task AddAsync(MenuItem item, CancellationToken cancellationToken = default) => await dbContext.MenuItems.AddAsync(item, cancellationToken);

    /// <summary>
    /// Updates the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        dbContext.MenuItems.Update(item);

        return Task.CompletedTask;
    }


    /// <summary>
    /// Deletes the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        dbContext.MenuItems.Remove(item);

        return Task.CompletedTask;
    }
}
