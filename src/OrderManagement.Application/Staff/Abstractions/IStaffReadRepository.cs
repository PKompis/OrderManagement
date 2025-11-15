namespace OrderManagement.Application.Staff.Abstractions;

/// <summary>
/// Main interface for read operations on staff
/// </summary>
public interface IStaffReadRepository
{
    /// <summary>
    /// Gets the by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<Domain.Staff.Entities.Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the available couriers asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyList<Domain.Staff.Entities.Staff>> GetAvailableCouriersAsync(CancellationToken cancellationToken = default);
}
