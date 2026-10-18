namespace VulnerableEmployeeApi.Models;

// A row in the Employees table.
// Note which fields are sensitive: Salary, NationalInsurance and
// PerformanceNotes should never leave the server for an unauthorised caller.
// In the vulnerable build the whole entity is returned straight to clients,
// which is the "excessive data exposure" flaw (see EmployeesController).
public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }                 // sensitive
    public string NationalInsurance { get; set; } = "";  // sensitive (PII)
    public string PerformanceNotes { get; set; } = "";   // sensitive
}
