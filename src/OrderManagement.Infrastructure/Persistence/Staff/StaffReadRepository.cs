using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Infrastructure.Persistence.Staff;

/// <summary>
/// Staff Read Repository
/// </summary>
/// <seealso cref="IStaffReadRepository" />
public sealed class StaffReadRepository(AppDbContext dbContext) : IStaffReadRepository
{
    /// <summary>
    /// Gets the by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Domain.Staff.Entities.Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return default;

        return await dbContext.Staff.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets the available couriers asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<IReadOnlyList<Domain.Staff.Entities.Staff>> GetAvailableCouriersAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Staff.AsNoTracking().Where(s => s.IsActive && s.Role == StaffRole.Delivery).ToListAsync(cancellationToken);
}
