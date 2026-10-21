using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureEmployeeApi.Data;
using SecureEmployeeApi.Models;

namespace SecureEmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    // FIX (broken function-level authorization): admin role is required.
    // FIX (excessive data exposure): UserDto returns no password/hash.
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllUsers() =>
        Ok(_db.Users.Select(u => new UserDto(u.Id, u.Username, u.Role)).ToList());
}
