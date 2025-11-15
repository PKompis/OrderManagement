using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Application.Menu.Abstractions;

/// <summary>
/// Main interface for read operations on menu
/// </summary>
public interface IMenuItemReadRepository
{
    /// <summary>
    /// Gets the menu by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all menu asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
