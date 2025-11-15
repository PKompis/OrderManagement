using OrderManagement.Application.Common.Models;

namespace OrderManagement.Application.Common.Abstractions;

/// <summary>
/// Token service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates the token.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="role">The role.</param>
    TokenResult GenerateToken(Guid userId, string role);
}
