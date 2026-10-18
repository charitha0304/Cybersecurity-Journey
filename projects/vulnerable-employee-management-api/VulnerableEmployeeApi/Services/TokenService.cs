using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VulnerableEmployeeApi.Models;

namespace VulnerableEmployeeApi.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string CreateToken(User user)
    {
        // VULN (weak JWT / insecure config): the signing key is a predictable phrase
        // committed in plain text in appsettings.json. Because the secret is not secret
        // (it is in source control) and is low-entropy, an attacker can take it and
        // forge a token with "role": "Admin". The fix uses a long RANDOM key loaded
        // from a secret store, never committed.
        // (Note: current Microsoft.IdentityModel enforces RFC 7518 and rejects HS256
        //  keys of 256 bits or fewer with IDX10720, so the key must be > 32 chars. The
        //  weakness here is therefore "disclosed + low-entropy", not "too short".)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("employeeId", user.EmployeeId?.ToString() ?? ""),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
