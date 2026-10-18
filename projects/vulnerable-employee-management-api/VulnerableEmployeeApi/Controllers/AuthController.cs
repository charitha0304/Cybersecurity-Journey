using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableEmployeeApi.Data;
using VulnerableEmployeeApi.Models;
using VulnerableEmployeeApi.Services;

namespace VulnerableEmployeeApi.Controllers;

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

    // POST /api/auth/login
    //
    // VULN 1 (SQL injection): the query is built by string concatenation and run
    //   with FromSqlRaw, so input goes straight into the SQL text. Payloads such as
    //   username = ' OR '1'='1' --   bypass the password check and log you in as the
    //   first user (admin). The fix parameterises the query AND stops matching on a
    //   plain-text password.
    //
    // VULN 2 (missing rate limiting): there is no throttling on this endpoint, so it
    //   can be brute-forced / credential-stuffed (Burp Intruder, a Postman loop).
    //
    // VULN 3 (excessive data exposure): on success the full User entity — including
    //   the plain-text password and role — is returned in the response body.
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var sql = $"SELECT * FROM Users WHERE Username = '{req.Username}' AND Password = '{req.Password}'";
        var user = _db.Users.FromSqlRaw(sql).AsEnumerable().FirstOrDefault();

        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokens.CreateToken(user);
        return Ok(new { token, user }); // leaks the whole user object
    }
}
