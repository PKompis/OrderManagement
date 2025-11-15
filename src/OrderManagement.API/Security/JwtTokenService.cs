using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.API.Options;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagement.API.Security;

/// <summary>
/// Jwt Token Service
/// </summary>
/// <seealso cref="ITokenService" />
public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    /// <summary>
    /// Generates the token.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="role">The role.</param>
    public TokenResult GenerateToken(Guid userId, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(options.Value.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("sub", userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return new TokenResult(jwt, expires, role);
    }
}