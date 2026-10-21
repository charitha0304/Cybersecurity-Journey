namespace SecureEmployeeApi.Models;

// Request bodies
public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UpdateEmployeeRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }
    public string NationalInsurance { get; set; } = "";
    public string PerformanceNotes { get; set; } = "";
}

// FIX (excessive data exposure): response shapes expose only the needed fields.
public record EmployeeDto(int Id, string FullName, string Email, string Department);

public record EmployeeDetailDto(
    int Id, string FullName, string Email, string Department,
    decimal Salary, string NationalInsurance, string PerformanceNotes);

public record UserDto(int Id, string Username, string Role);      // no password hash

public record UserProfileDto(string Username, string Role);

public record LoginResponse(string Token, UserProfileDto User);   // no user entity, no password
