using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Application.Menu.Abstractions;

/// <summary>
/// Main interface for menu general operations
/// </summary>
public interface IMenuItemRepository
{
    /// <summary>
    /// Gets the menu by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds to the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(MenuItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the menu asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default);
}
