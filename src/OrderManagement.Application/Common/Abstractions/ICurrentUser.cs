namespace OrderManagement.Application.Common.Abstractions;

/// <summary>
/// Represents the currently authenticated user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the user identifier (Customer or Staff id), or null if anonymous.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the role of the current user (e.g. Customer, Staff, Courier, Admin).
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
