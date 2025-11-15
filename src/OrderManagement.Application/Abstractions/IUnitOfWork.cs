namespace OrderManagement.Application.Abstractions;

/// <summary>
/// Unit of work interface
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves the changes asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
