using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Enums;

namespace WMS.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(WmsDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Employees.AnyAsync())
            return;

        await SeedRolesAsync(context);
        await SeedDepartmentsAsync(context);
        await SeedUsersAndEmployeesAsync(context);
        await SeedClientsAsync(context);
        await SeedProjectsAsync(context);
        await SeedProjectAllocationsAsync(context);
        await SeedAttendanceAsync(context);
        await SeedLeavesAsync(context);
        await SeedAnnouncementsAsync(context);
    }

    private static async Task SeedRolesAsync(
        WmsDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        await context.Roles.AddRangeAsync(
            new Role
            {
                RoleName = "Admin",
                Description = "System Administrator"
            },
            new Role
            {
                RoleName = "Manager",
                Description = "Team Manager"
            },
            new Role
            {
                RoleName = "Employee",
                Description = "Standard Employee"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedDepartmentsAsync(
        WmsDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;

        await context.Departments.AddRangeAsync(
            new Department
            {
                DepartmentName = "Engineering",
                Description = "Engineering Department"
            },
            new Department
            {
                DepartmentName = "Human Resources",
                Description = "HR Department"
            },
            new Department
            {
                DepartmentName = "Finance",
                Description = "Finance Department"
            },
            new Department
            {
                DepartmentName = "Operations",
                Description = "Operations Department"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAndEmployeesAsync(
        WmsDbContext context)
    {
        var managerRoleId =
            await context.Roles
                .Where(r => r.RoleName == "Manager")
                .Select(r => r.RoleId)
                .FirstAsync();

        var employeeRoleId =
            await context.Roles
                .Where(r => r.RoleName == "Employee")
                .Select(r => r.RoleId)
                .FirstAsync();

        var departmentIds =
            await context.Departments
                .Select(d => d.DepartmentId)
                .ToListAsync();

        var employees = new List<Employee>();

        for (int i = 1; i <= 15; i++)
        {
            employees.Add(new Employee
            {
                FirstName = $"Employee{i}",
                LastName = "Demo",
                Email = $"employee{i}@wms.com",
                PhoneNumber = $"987654{i:0000}",
                Gender = i % 2 == 0 ? 'F' : 'M',
                DOB = new DateOnly(1995, 1, (i % 28) + 1),
                DOJ = DateOnly.FromDateTime(
                    DateTime.UtcNow.AddMonths(-(i + 2))),
                DepartmentId =
                    departmentIds[i % departmentIds.Count],
                RoleId =
                    i <= 3
                        ? managerRoleId
                        : employeeRoleId,
                Status =
                    i % 5 == 0
                        ? EmployeeStatus.Inactive
                        : EmployeeStatus.Active
            });
        }

        await context.Employees.AddRangeAsync(employees);

        await context.SaveChangesAsync();

        var adminRoleId =
            await context.Roles
                .Where(r => r.RoleName == "Admin")
                .Select(r => r.RoleId)
                .FirstAsync();

        var logins = new List<UserLogin>
        {
            new UserLogin
            {
                Username = "admin",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        "Admin@123"),
                RoleId = adminRoleId,
                MustChangePassword = false
            }
        };

        foreach (var employee in employees)
        {
            logins.Add(new UserLogin
            {
                Username = employee.Email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        "Password@123"),
                RoleId = employee.RoleId,
                EmployeeId = employee.EmployeeId,
                MustChangePassword = false
            });
        }

        await context.UserLogins.AddRangeAsync(logins);

        await context.SaveChangesAsync();
    }

    private static async Task SeedClientsAsync(
        WmsDbContext context)
    {
        if (await context.Clients.AnyAsync())
            return;

        await context.Clients.AddRangeAsync(
            new Client
            {
                ClientName = "Microsoft",
                ClientLocation = "USA",
                Status = true
            },
            new Client
            {
                ClientName = "Google",
                ClientLocation = "USA",
                Status = true
            },
            new Client
            {
                ClientName = "Amazon",
                ClientLocation = "USA",
                Status = true
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedProjectsAsync(
        WmsDbContext context)
    {
        if (await context.Projects.AnyAsync())
            return;

        var clients =
            await context.Clients.ToListAsync();

        await context.Projects.AddRangeAsync(
            new Project
            {
                ProjectName = "ERP System",
                ClientId = clients[0].ClientId,
                Status = ProjectStatus.Active
            },
            new Project
            {
                ProjectName = "AI Analytics",
                ClientId = clients[1].ClientId,
                Status = ProjectStatus.Active
            },
            new Project
            {
                ProjectName = "HR Portal",
                ClientId = clients[2].ClientId,
                Status = ProjectStatus.OnHold
            },
            new Project
            {
                ProjectName = "Inventory System",
                ClientId = clients[0].ClientId,
                Status = ProjectStatus.Completed
            },
            new Project
            {
                ProjectName = "Finance Dashboard",
                ClientId = clients[1].ClientId,
                Status = ProjectStatus.Active
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedProjectAllocationsAsync(
        WmsDbContext context)
    {
        if (await context.EmployeeProjects.AnyAsync())
            return;

        var employees =
            await context.Employees.ToListAsync();

        var projects =
            await context.Projects.ToListAsync();

        var allocations = new List<EmployeeProject>();

        for (int i = 0; i < employees.Count; i++)
        {
            allocations.Add(new EmployeeProject
            {
                EmpId = employees[i].EmployeeId,
                ProjectId =
                    projects[i % projects.Count].ProjectId,
                AssignedOn =
                    DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedBy = "System"
            });
        }

        await context.EmployeeProjects
            .AddRangeAsync(allocations);

        await context.SaveChangesAsync();
    }

    private static async Task SeedAttendanceAsync(
        WmsDbContext context)
    {
        if (await context.Attendances.AnyAsync())
            return;

        var employees =
            await context.Employees.ToListAsync();

        var attendances = new List<Attendance>();

        foreach (var employee in employees)
        {
            for (int day = 1; day <= 20; day++)
            {
                if (day % 6 == 0)
                    continue;

                var date =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(-day));

                var checkIn =
                    date.ToDateTime(
                        new TimeOnly(
                            9,
                            Random.Shared.Next(0, 30)));

                var checkOut =
                    date.ToDateTime(
                        new TimeOnly(
                            18,
                            Random.Shared.Next(0, 30)));

                attendances.Add(new Attendance
                {
                    EmpId = employee.EmployeeId,
                    AttendanceDate = date,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    WorkMode =
                        (WorkMode)
                        Random.Shared.Next(1, 4)
                });
            }
        }

        await context.Attendances
            .AddRangeAsync(attendances);

        await context.SaveChangesAsync();
    }

    private static async Task SeedLeavesAsync(
        WmsDbContext context)
    {
        if (await context.Leaves.AnyAsync())
            return;

        var employees =
            await context.Employees.Take(3).ToListAsync();

        await context.Leaves.AddRangeAsync(
            new Leave
            {
                EmpId = employees[0].EmployeeId,
                LeaveType = LeaveType.Sick,
                Reason = "Fever",
                FromDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(-3)),
                ToDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(-1)),
                Status = LeaveStatus.Approved
            },

            new Leave
            {
                EmpId = employees[1].EmployeeId,
                LeaveType = LeaveType.Casual,
                Reason = "Personal Work",
                FromDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(2)),
                ToDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(4)),
                Status = LeaveStatus.Pending
            },

            new Leave
            {
                EmpId = employees[2].EmployeeId,
                LeaveType = LeaveType.Earned,
                Reason = "Vacation",
                FromDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(-10)),
                ToDate =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow.AddDays(-5)),
                Status = LeaveStatus.Rejected
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedAnnouncementsAsync(
        WmsDbContext context)
    {
        if (await context.Announcements.AnyAsync())
            return;

        var adminEmployee =
            await context.Employees.FirstOrDefaultAsync();

        await context.Announcements.AddRangeAsync(
            new Announcement
            {
                Title = "Welcome",
                Message =
                    "Welcome to WMS Portal",
                CreatedBy =
                    adminEmployee?.EmployeeId ?? 1,
                IsActive = true
            },

            new Announcement
            {
                Title = "Holiday Notice",
                Message =
                    "Office closed this Friday",
                CreatedBy =
                    adminEmployee?.EmployeeId ?? 1,
                IsActive = true
            },

            new Announcement
            {
                Title = "Project Release",
                Message =
                    "ERP v2 releasing next week",
                CreatedBy =
                    adminEmployee?.EmployeeId ?? 1,
                IsActive = true
            });

        await context.SaveChangesAsync();
    }
}
