using Microsoft.EntityFrameworkCore;
using SecureEmployeeApi.Models;

namespace SecureEmployeeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Employee> Employees => Set<Employee>();
}
