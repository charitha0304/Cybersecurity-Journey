namespace SecureEmployeeApi.Models;

// FIX (excessive data exposure + weak auth): the password is stored as a salted
// PBKDF2 hash, never in plain text. Nothing in this entity should ever be
// serialized directly to a client — responses use DTOs instead.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Employee"; // "Admin" or "Employee"
    public int? EmployeeId { get; set; }
}
