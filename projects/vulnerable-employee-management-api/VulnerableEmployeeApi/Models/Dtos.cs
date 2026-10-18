namespace VulnerableEmployeeApi.Models;

// Request body for POST /api/auth/login
public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

// Request body for PUT /api/employees/{id}
public class UpdateEmployeeRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }
    public string NationalInsurance { get; set; } = "";
    public string PerformanceNotes { get; set; } = "";
}
