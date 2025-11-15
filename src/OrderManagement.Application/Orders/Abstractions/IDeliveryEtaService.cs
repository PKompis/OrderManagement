using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.Application.Orders.Abstractions;

/// <summary>
/// Main interface for calculation eta
/// </summary>
public interface IDeliveryEtaService
{
    /// <summary>
    /// Calculates the estimated travel time for a delivery order based on its address.
    /// </summary>
    Task<DeliveryEstimate?> GetEstimateAsync(DeliveryAddress address, CancellationToken cancellationToken = default);
}
