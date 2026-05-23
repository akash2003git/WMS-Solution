using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly WmsDbContext _context;

    public DepartmentRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .Include(d => d.Employees)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int departmentId)
    {
        return await _context.Departments
            .Include(d => d.Employees)
            .ThenInclude(e => e.Role)
            .FirstOrDefaultAsync(d =>
                d.DepartmentId == departmentId);
    }

    public async Task<bool> DepartmentNameExistsAsync(
        string departmentName)
    {
        return await _context.Departments
            .AnyAsync(d =>
                d.DepartmentName.ToLower()
                    == departmentName.ToLower());
    }

    public async Task<bool> DepartmentNameExistsAsync(
        string departmentName,
        int excludeDepartmentId)
    {
        return await _context.Departments
            .AnyAsync(d =>
                d.DepartmentId != excludeDepartmentId
                &&
                d.DepartmentName.ToLower()
                    == departmentName.ToLower());
    }

    public async Task<bool> HasEmployeesAsync(int departmentId)
    {
        return await _context.Employees
            .AnyAsync(e => e.DepartmentId == departmentId);
    }

    public async Task CreateDepartmentAsync(
        Department department)
    {
        await _context.Departments.AddAsync(department);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateDepartmentAsync(
        Department department)
    {
        _context.Departments.Update(department);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDepartmentAsync(
        Department department)
    {
        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();
    }
}
