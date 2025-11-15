namespace OrderManagement.Contracts.Auth.Requests;

/// <summary>
/// Login Request
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// The user identifier (Customer.Id or Staff.Id depending on IsStaff).
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Indicates whether the user is a staff member (Kitchen/Delivery/Admin) or a customer.
    /// </summary>
    public bool IsStaff { get; init; }
}
