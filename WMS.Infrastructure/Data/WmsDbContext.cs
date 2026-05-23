using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data;

public class WmsDbContext(DbContextOptions<WmsDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Leave> Leaves => Set<Leave>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
            new Role { RoleId = 2, RoleName = "Manager", Description = "Team Manager" },
            new Role { RoleId = 3, RoleName = "Employee", Description = "Standard Employee" }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department
            {
                DepartmentId = 1,
                DepartmentName = "Human Resources",
                Description = "HR Department"
            },
            new Department
            {
                DepartmentId = 2,
                DepartmentName = "Engineering",
                Description = "Engineering Department"
            },
            new Department
            {
                DepartmentId = 3,
                DepartmentName = "Finance",
                Description = "Finance Department"
            },
            new Department
            {
                DepartmentId = 4,
                DepartmentName = "Operations",
                Description = "Operations Department"
            }
        );

        // Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin@123"));

        modelBuilder.Entity<UserLogin>().HasData(
            new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "$2a$11$iBjFJvLHdAO92u5Woyz9d.mL3urAktHEJYr4YS4NTZaTfU2O3aBZS",
                RoleId = 1,
                LastLogin = null
            }
        );

        modelBuilder.Entity<Attendance>()
            .HasIndex(a => new
            {
                a.EmpId,
                a.AttendanceDate
            })
            .IsUnique();

        modelBuilder.Entity<UserLogin>()
            .HasOne(u => u.Employee)
            .WithOne(e => e.UserLogin)
            .HasForeignKey<UserLogin>(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .ToTable(t => t
                .HasCheckConstraint("CK_Employee_Gender", "Gender IN ('M','F','O')"));

        modelBuilder.Entity<Employee>()
          .HasIndex(e => e.Email)
          .IsUnique();

        modelBuilder.Entity<UserLogin>()
          .HasIndex(u => u.Username)
          .IsUnique();

        modelBuilder.Entity<Attendance>()
          .Property(a => a.TotalHours)
          .HasComputedColumnSql("CAST(DATEDIFF(MINUTE, CheckIn, CheckOut) AS FLOAT) / 60.0", stored: true);

        modelBuilder.Entity<AuditLog>()
            .Property(a => a.Action)
            .HasConversion<string>();

        modelBuilder.Entity<Employee>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Leave>()
            .Property(l => l.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Leave>()
            .Property(l => l.LeaveType)
            .HasConversion<string>();

        modelBuilder.Entity<Project>()
            .Property(p => p.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Attendance>()
            .Property(a => a.WorkMode)
            .HasConversion<string>();

        modelBuilder.Entity<Attendance>()
          .HasOne(a => a.Employee)
          .WithMany(e => e.Attendances)
          .HasForeignKey(a => a.EmpId)
          .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Leave>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.Leaves)
            .HasForeignKey(l => l.EmpId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeProject>()
            .HasOne(ep => ep.Employee)
            .WithMany(e => e.ProjectAllocations)
            .HasForeignKey(ep => ep.EmpId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeProject>()
                .HasOne(ep => ep.Project)
                .WithMany(p => p.EmployeeAllocations)
                .HasForeignKey(ep => ep.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Employee>()
               .HasOne(e => e.Department)
               .WithMany(d => d.Employees)
               .HasForeignKey(e => e.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
              .HasOne(e => e.Role)
              .WithMany(r => r.Employees)
              .HasForeignKey(e => e.RoleId)
              .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserLogin>()
            .HasOne(u => u.Role)
            .WithMany(r => r.UserLogins)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.Creator)
            .WithMany()
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
