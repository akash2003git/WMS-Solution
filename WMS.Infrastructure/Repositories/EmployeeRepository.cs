using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly WmsDbContext _context;

    public EmployeeRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.UserLogins
            .AnyAsync(u => u.Username == username);
    }

    public async Task CreateEmployeeAsync(
        Employee employee,
        UserLogin userLogin)
    {
        await _context.Employees.AddAsync(employee);

        await _context.SaveChangesAsync();

        userLogin.EmployeeId = employee.EmployeeId;

        await _context.UserLogins.AddAsync(userLogin);

        await _context.SaveChangesAsync();
    }

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<Employee?> GetEmployeeByEmailAsync(string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .ToListAsync();
    }

    public async Task<bool> DepartmentExistsAsync(int departmentId)
    {
        return await _context.Departments
            .AnyAsync(d => d.DepartmentId == departmentId);
    }

    public async Task<bool> RoleExistsAsync(int roleId)
    {
        return await _context.Roles
            .AnyAsync(r => r.RoleId == roleId);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        _context.Employees.Update(employee);

        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
