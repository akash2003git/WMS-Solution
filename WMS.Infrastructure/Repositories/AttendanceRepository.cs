using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly WmsDbContext _context;

    public AttendanceRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateOnly date)
    {
        return await _context.Attendances
            .FirstOrDefaultAsync(a => a.EmpId == employeeId && a.AttendanceDate == date);
    }

    public async Task AddAttendanceAsync(Attendance attendance)
    {
        await _context.Attendances.AddAsync(attendance);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAttendanceAsync(Attendance attendance)
    {
        _context.Attendances.Update(attendance);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Attendance>> GetAttendancesAsync(int? employeeId = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(a => a.EmpId == employeeId.Value);

        if (fromDate.HasValue)
            query = query.Where(a => a.AttendanceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.AttendanceDate <= toDate.Value);

        return await query.ToListAsync();
    }

    public async Task<List<Employee>> GetAbsenteesAsync(DateOnly date)
    {
        var attendedEmployeeIds = await _context.Attendances
            .Where(a => a.AttendanceDate == date)
            .Select(a => a.EmpId)
            .ToListAsync();

        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Where(e => !attendedEmployeeIds.Contains(e.EmployeeId))
            .ToListAsync();
    }
}
