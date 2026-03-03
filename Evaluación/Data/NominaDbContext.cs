using Microsoft.EntityFrameworkCore;
using Evaluación.Models;

namespace Evaluación.Data
{
    public class NominaDbContext : DbContext
    {
        public NominaDbContext(DbContextOptions<NominaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<DeptEmp> DeptEmp { get; set; }
    }
}