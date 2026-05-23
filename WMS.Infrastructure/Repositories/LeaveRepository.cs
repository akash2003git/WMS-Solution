using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly WmsDbContext _context;

    public LeaveRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task AddLeaveAsync(Leave leave)
    {
        await _context.Leaves.AddAsync(leave);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateLeaveAsync(Leave leave)
    {
        _context.Leaves.Update(leave);

        await _context.SaveChangesAsync();
    }

    public async Task<Leave?> GetByIdAsync(int leaveId)
    {
        return await _context.Leaves
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l =>
                l.LeaveId == leaveId);
    }

    public async Task<List<Leave>> GetLeavesAsync(
        int? employeeId = null,
        LeaveStatus? status = null)
    {
        var query = _context.Leaves
            .Include(l => l.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
        {
            query = query.Where(l =>
                l.EmpId == employeeId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(l =>
                l.Status == status.Value);
        }

        return await query
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingLeaveAsync(
        int employeeId,
        DateOnly fromDate,
        DateOnly toDate)
    {
        return await _context.Leaves.AnyAsync(l =>
            l.EmpId == employeeId
            &&
            (
                l.Status == LeaveStatus.Pending
                ||
                l.Status == LeaveStatus.Approved
            )
            &&
            l.FromDate <= toDate
            &&
            l.ToDate >= fromDate);
    }
}
