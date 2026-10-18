namespace VulnerableEmployeeApi.Models;

// A login account.
// VULN (insecure design): the password is stored in plain text so the
// SQL-injection login bypass and the "excessive data exposure" demo stay
// easy to follow. Real systems must store a salted hash (see the fixed build).
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";   // plain text — intentionally wrong
    public string Role { get; set; } = "Employee"; // "Admin" or "Employee"
    public int? EmployeeId { get; set; }           // which Employee row this user "owns"
}
