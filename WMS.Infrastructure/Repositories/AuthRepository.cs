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

    public async Task UpdateUserAsync(UserLogin user)
    {
        _context.UserLogins.Update(user);
        await _context.SaveChangesAsync();
    }
}
