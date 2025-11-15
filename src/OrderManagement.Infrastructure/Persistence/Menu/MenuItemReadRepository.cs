using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Infrastructure.Persistence.Menu;

/// <summary>
/// Menu Item Read Repository
/// </summary>
/// <seealso cref="IMenuItemReadRepository" />
public sealed class MenuItemReadRepository(AppDbContext dbContext) : IMenuItemReadRepository
{
    /// <summary>
    /// Gets the menu by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return default;

        return await dbContext.MenuItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get all menu asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.MenuItems.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
}