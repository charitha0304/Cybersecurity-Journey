using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VulnerableEmployeeApi.Data;

namespace VulnerableEmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    // GET /api/admin/users
    //
    // VULN (broken function-level authorization): this is an admin-only report, but
    // the gate is only [Authorize] — any authenticated user (e.g. alice) can call it.
    // It should be [Authorize(Roles = "Admin")].
    //
    // VULN (excessive data exposure): it also returns every user's plain-text
    // password and role, handing an attacker the full credential set.
    [HttpGet("users")]
    [Authorize]
    public IActionResult GetAllUsers()
    {
        return Ok(_db.Users.ToList());
    }
}
