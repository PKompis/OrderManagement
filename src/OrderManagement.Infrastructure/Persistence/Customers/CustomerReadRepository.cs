using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Customers.Abstractions;

namespace OrderManagement.Infrastructure.Persistence.Customers;

/// <summary>
/// Customer Read Repository
/// </summary>
/// <seealso cref="ICustomerReadRepository" />
public sealed class CustomerReadRepository(AppDbContext dbContext) : ICustomerReadRepository
{
    /// <summary>
    /// Whether customer exists asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return false;

        return await dbContext.Customers.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken);
    }
}
