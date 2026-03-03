using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Models;

namespace SistemaNomina.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets para cada modelo
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DeptEmp> DeptEmps { get; set; }
        public DbSet<DeptManager> DeptManagers { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LogAuditoriaSalario> LogAuditoriaSalarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CONFIGURACIÓN DE CLAVES COMPUESTAS
            modelBuilder.Entity<DeptEmp>()
                .HasKey(de => new { de.emp_no, de.dept_no, de.from_date });

            modelBuilder.Entity<DeptManager>()
                .HasKey(dm => new { dm.emp_no, dm.dept_no, dm.from_date });

            modelBuilder.Entity<Salary>()
                .HasKey(s => new { s.emp_no, s.from_date });

            modelBuilder.Entity<Title>()
                .HasKey(t => new { t.emp_no, t.title, t.from_date });

            // RELACIONES
            modelBuilder.Entity<DeptEmp>()
                .HasOne(de => de.Employee)
                .WithMany(e => e.DeptEmps)
                .HasForeignKey(de => de.emp_no);

            modelBuilder.Entity<DeptEmp>()
                .HasOne(de => de.Department)
                .WithMany(d => d.DeptEmps)
                .HasForeignKey(de => de.dept_no);

            modelBuilder.Entity<Salary>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(s => s.emp_no);

            modelBuilder.Entity<Title>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.Titles)
                .HasForeignKey(t => t.emp_no);

            modelBuilder.Entity<DeptManager>()
                .HasOne(dm => dm.Employee)
                .WithMany(e => e.DeptManagers)
                .HasForeignKey(dm => dm.emp_no);

            modelBuilder.Entity<DeptManager>()
                .HasOne(dm => dm.Department)
                .WithMany(d => d.DeptManagers)
                .HasForeignKey(dm => dm.dept_no);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.emp_no);

            // ÍNDICES ÚNICOS
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.ci)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.correo)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.dept_name)
                .IsUnique();

            // DATOS SEMILLA - DEPARTAMENTOS
            modelBuilder.Entity<Department>().HasData(
                new Department { dept_no = "IT", dept_name = "Tecnología" },
                new Department { dept_no = "HR", dept_name = "Recursos Humanos" },
                new Department { dept_no = "FIN", dept_name = "Finanzas" },
                new Department { dept_no = "MKT", dept_name = "Marketing" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}