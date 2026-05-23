using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly WmsDbContext _context;

    public AuthRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<UserLogin?> GetByUsernameAsync(string username)
    {
        return await _context.UserLogins
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserLogin?> GetByIdAsync(int userId)
    {
        return await _context.UserLogins
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _context.UserLogins.FindAsync(userId);

        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.UserLogins.AnyAsync(u => u.Username == username);
    }

    public async Task CreateEmployeeAsync(Employee employee, UserLogin userLogin)
    {
        await _context.Employees.AddAsync(employee);

        await _context.SaveChangesAsync();

        userLogin.EmployeeId = employee.EmployeeId;

        await _context.UserLogins.AddAsync(userLogin);

        await _context.SaveChangesAsync();
    }
}
