namespace OrderManagement.Application.Customers.Abstractions;

/// <summary>
/// Main interface for read operations on customers
/// </summary>
public interface ICustomerReadRepository
{
    /// <summary>
    /// Whether customer exists asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
