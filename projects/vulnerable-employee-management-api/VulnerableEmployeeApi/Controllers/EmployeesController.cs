using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableEmployeeApi.Data;
using VulnerableEmployeeApi.Models;

namespace VulnerableEmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any authenticated user — note there is no role or ownership check below
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    // GET /api/employees
    //
    // VULN (excessive data exposure): returns every column of every employee —
    // salaries, National Insurance numbers, performance notes — to any logged-in
    // user. The fix projects to a DTO that omits sensitive fields.
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_db.Employees.ToList());
    }

    // GET /api/employees/{id}
    //
    // VULN (IDOR / broken object-level authorization): the record is looked up by
    // id with no check that the caller is allowed to see THAT employee. A logged-in
    // user can read anyone's record by changing the id (e.g. alice -> /api/employees/5
    // reads the Head of Engineering's confidential pay). The fix compares the id
    // against the caller's own "employeeId" claim (or requires the Admin role).
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var employee = _db.Employees.Find(id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    // GET /api/employees/search?name=...
    //
    // VULN (SQL injection, UNION-based): the LIKE clause is concatenated into raw SQL.
    // A payload such as
    //   %' UNION SELECT Id, Username, Password, Role, 0, '', '' FROM Users --
    // pulls usernames and plain-text passwords out through the employee columns.
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string name)
    {
        var sql = $"SELECT * FROM Employees WHERE FullName LIKE '%{name}%'";
        var results = _db.Employees.FromSqlRaw(sql).AsEnumerable().ToList();
        return Ok(results);
    }

    // PUT /api/employees/{id}
    //
    // VULN (broken authorization + mass assignment): any authenticated user can edit
    // any employee, including their own Salary and the role-sensitive fields. There is
    // no ownership check and no Admin requirement.
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest req)
    {
        var employee = _db.Employees.Find(id);
        if (employee == null) return NotFound();

        employee.FullName = req.FullName;
        employee.Email = req.Email;
        employee.Department = req.Department;
        employee.Salary = req.Salary;
        employee.NationalInsurance = req.NationalInsurance;
        employee.PerformanceNotes = req.PerformanceNotes;
        _db.SaveChanges();

        return Ok(employee);
    }

    // DELETE /api/employees/{id}
    //
    // VULN (broken function-level authorization): deleting an employee is an
    // admin-only action, but the only gate is [Authorize] at the class level, so any
    // logged-in user can delete any record. The fix adds [Authorize(Roles = "Admin")].
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var employee = _db.Employees.Find(id);
        if (employee == null) return NotFound();

        _db.Employees.Remove(employee);
        _db.SaveChanges();
        return NoContent();
    }
}
