using OrderManagement.Application.Common.Abstractions;
using System.Security.Claims;

namespace OrderManagement.API.Security;

/// <summary>
/// Current User
/// </summary>
/// <seealso cref="ICurrentUser" />
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the user identifier (Customer or Staff id), or null if anonymous.
    /// </summary>
    public Guid? UserId
    {
        get
        {
            if (!IsAuthenticated) return null;

            var id = User!.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");

            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    /// <summary>
    /// Gets the role of the current user (e.g. Customer, Staff, Courier, Admin).
    /// </summary>
    public string? Role
    {
        get
        {
            if (!IsAuthenticated) return null;

            // Standard role claim
            return User!.FindFirstValue(ClaimTypes.Role);
        }
    }
}
