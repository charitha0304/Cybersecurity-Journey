using Microsoft.EntityFrameworkCore;
using VulnerableEmployeeApi.Models;

namespace VulnerableEmployeeApi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();
        if (db.Users.Any()) return;

        var employees = new List<Employee>
        {
            new Employee { FullName = "Alice Morgan",  Email = "alice@acme.test",  Department = "Engineering", Salary = 48000m, NationalInsurance = "AB123456C", PerformanceNotes = "Solid mid-level engineer. Due for review in Q1." },
            new Employee { FullName = "Bob Chen",       Email = "bob@acme.test",    Department = "Engineering", Salary = 52000m, NationalInsurance = "CD234567D", PerformanceNotes = "Strong performer. Flight risk — counter-offer expected." },
            new Employee { FullName = "Carol Diaz",     Email = "carol@acme.test",  Department = "Finance",     Salary = 61000m, NationalInsurance = "EF345678E", PerformanceNotes = "Handles payroll. On a performance improvement plan." },
            new Employee { FullName = "David Okafor",   Email = "david@acme.test",  Department = "HR",          Salary = 45000m, NationalInsurance = "GH456789F", PerformanceNotes = "New joiner. Probation ends next month." },
            new Employee { FullName = "Priya Nair",     Email = "priya@acme.test",  Department = "Management",  Salary = 90000m, NationalInsurance = "IJ567890G", PerformanceNotes = "Head of Engineering. Confidential comp package." },
        };
        db.Employees.AddRange(employees);
        db.SaveChanges();

        var users = new List<User>
        {
            new User { Username = "admin", Password = "Admin@123", Role = "Admin",    EmployeeId = 5 }, // Priya
            new User { Username = "alice", Password = "Password1", Role = "Employee", EmployeeId = 1 },
            new User { Username = "bob",   Password = "Hunter2",   Role = "Employee", EmployeeId = 2 },
            new User { Username = "carol", Password = "Finance99", Role = "Employee", EmployeeId = 3 },
        };
        db.Users.AddRange(users);
        db.SaveChanges();
    }
}
