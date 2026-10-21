using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SecureEmployeeApi.Data;
using SecureEmployeeApi.Models;
using SecureEmployeeApi.Services;

namespace SecureEmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    public AuthController(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    // FIX (SQL injection): the user is looked up with a parameterised LINQ query,
    //   so input is always data and can never change the query.
    // FIX (weak auth / login bypass): the password is verified against a salted hash.
    // FIX (excessive data exposure): the response returns only a token and a minimal
    //   profile — never the user entity or any password.
    // FIX (missing rate limiting): the endpoint is throttled (see Program.cs).
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == req.Username);

        if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokens.CreateToken(user);
        return Ok(new LoginResponse(token, new UserProfileDto(user.Username, user.Role)));
    }
}
