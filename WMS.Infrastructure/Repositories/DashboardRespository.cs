using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class DashboardRepository
    : IDashboardRepository
{
    private readonly WmsDbContext _context;

    public DashboardRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<int>
        GetTotalEmployeesAsync()
    {
        return await _context.Employees.CountAsync();
    }

    public async Task<int>
        GetTotalDepartmentsAsync()
    {
        return await _context.Departments.CountAsync();
    }

    public async Task<int>
        GetEmployeesOnLeaveTodayAsync()
    {
        var today =
            DateOnly.FromDateTime(DateTime.Today);

        return await _context.Leaves
            .CountAsync(l =>
                l.Status == LeaveStatus.Approved &&
                today >= l.FromDate &&
                today <= l.ToDate);
    }

    public async Task<int>
        GetAttendanceTodayAsync()
    {
        var today =
            DateOnly.FromDateTime(DateTime.Today);

        return await _context.Attendances
            .CountAsync(a =>
                a.AttendanceDate == today);
    }

    public async Task<int>
        GetActiveProjectsAsync()
    {
        return await _context.Projects
            .CountAsync(p =>
                p.Status == ProjectStatus.Active);
    }

    public async Task<int>
        GetInactiveEmployeesAsync()
    {
        return await _context.Employees
            .CountAsync(e =>
                e.Status == EmployeeStatus.Inactive);
    }

    public async Task<int>
        GetPendingLeaveRequestsAsync()
    {
        return await _context.Leaves
            .CountAsync(l =>
                l.Status == LeaveStatus.Pending);
    }

    public async Task<int>
        GetActiveProjectAllocationsAsync()
    {
        return await _context.EmployeeProjects
            .CountAsync(ep => ep.Status);
    }

    public async Task<int>
        GetEmployeeAttendanceThisMonthAsync(
            int employeeId)
    {
        var today = DateTime.Today;

        return await _context.Attendances
            .CountAsync(a =>
                a.EmpId == employeeId &&
                a.AttendanceDate.Month == today.Month &&
                a.AttendanceDate.Year == today.Year);
    }

    public async Task<int>
        GetEmployeeLeaveCountThisMonthAsync(
            int employeeId)
    {
        var today = DateTime.Today;

        return await _context.Leaves
            .CountAsync(l =>
                l.EmpId == employeeId &&
                l.Status == LeaveStatus.Approved &&
                l.FromDate.Month == today.Month &&
                l.FromDate.Year == today.Year);
    }

    public async Task<List<Project>>
        GetEmployeeProjectsAsync(
            int employeeId)
    {
        return await _context.EmployeeProjects
            .Where(ep =>
                ep.EmpId == employeeId &&
                ep.Status)
            .Include(ep => ep.Project)
            .Select(ep => ep.Project!)
            .ToListAsync();
    }

    public async Task<int>
        GetActiveEmployeesAsync()
    {
        return await _context.Employees
            .CountAsync(e =>
                e.Status == EmployeeStatus.Active);
    }
}
