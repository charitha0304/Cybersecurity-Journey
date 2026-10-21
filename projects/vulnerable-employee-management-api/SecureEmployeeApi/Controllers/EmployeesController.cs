using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureEmployeeApi.Data;
using SecureEmployeeApi.Models;

namespace SecureEmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    private bool IsAdmin => User.IsInRole("Admin");
    private int CallerEmployeeId =>
        int.TryParse(User.FindFirstValue("employeeId"), out var e) ? e : -1;

    // FIX (excessive data exposure): the list returns a minimal DTO — no salary,
    // NI number, or performance notes.
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_db.Employees
            .Select(e => new EmployeeDto(e.Id, e.FullName, e.Email, e.Department))
            .ToList());

    // FIX (IDOR / BOLA): a standard user may read only their own record; admins may
    // read any. The ownership check happens before anything is returned.
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (!IsAdmin && id != CallerEmployeeId)
            return Forbid();

        var e = _db.Employees.Find(id);
        if (e is null) return NotFound();

        return Ok(new EmployeeDetailDto(
            e.Id, e.FullName, e.Email, e.Department, e.Salary, e.NationalInsurance, e.PerformanceNotes));
    }

    // FIX (SQL injection): parameterised LIKE via EF.Functions — the term is bound,
    // not concatenated. Returns a minimal DTO (no sensitive fields).
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name)
    {
        var term = name ?? "";
        var results = _db.Employees
            .Where(e => EF.Functions.Like(e.FullName, $"%{term}%"))
            .Select(e => new EmployeeDto(e.Id, e.FullName, e.Email, e.Department))
            .ToList();
        return Ok(results);
    }

    // FIX (broken authorization): editing an employee is admin-only.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest req)
    {
        var e = _db.Employees.Find(id);
        if (e is null) return NotFound();

        e.FullName = req.FullName;
        e.Email = req.Email;
        e.Department = req.Department;
        e.Salary = req.Salary;
        e.NationalInsurance = req.NationalInsurance;
        e.PerformanceNotes = req.PerformanceNotes;
        _db.SaveChanges();

        return Ok(new EmployeeDetailDto(
            e.Id, e.FullName, e.Email, e.Department, e.Salary, e.NationalInsurance, e.PerformanceNotes));
    }

    // FIX (broken authorization): deleting an employee is admin-only.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var e = _db.Employees.Find(id);
        if (e is null) return NotFound();

        _db.Employees.Remove(e);
        _db.SaveChanges();
        return NoContent();
    }
}
