using OrderManagement.Application.Common.Abstractions;

namespace OrderManagement.Infrastructure.Persistence;

/// <summary>
/// Unit Of Work Pattern
/// </summary>
/// <seealso cref="IUnitOfWork" />
public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    /// <summary>
    /// Saves the changes asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
