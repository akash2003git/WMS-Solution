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
}
