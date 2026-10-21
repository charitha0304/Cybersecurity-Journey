namespace SecureEmployeeApi.Models;

// The entity still holds sensitive fields (Salary, NationalInsurance,
// PerformanceNotes), but they are never returned directly — controllers project
// to DTOs that expose only what the caller is entitled to.
public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }
    public string NationalInsurance { get; set; } = "";
    public string PerformanceNotes { get; set; } = "";
}
