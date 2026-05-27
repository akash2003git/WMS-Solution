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
            .Include(a => a.Employee)
            .CountAsync(a =>
                a.AttendanceDate == today
                &&
                a.Employee != null
                &&
                a.Employee.Status == EmployeeStatus.Active);
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
        GetEmployeeLeaveDaysThisMonthAsync(
            int employeeId)
    {
        var today = DateTime.Today;

        var startOfMonth =
            new DateOnly(today.Year, today.Month, 1);

        var endOfMonth =
            new DateOnly(
                today.Year,
                today.Month,
                DateTime.DaysInMonth(today.Year, today.Month));

        var leaves =
            await _context.Leaves
                .Where(l =>
                    l.EmpId == employeeId &&
                    l.Status == LeaveStatus.Approved &&
                    l.FromDate <= endOfMonth &&
                    l.ToDate >= startOfMonth)
                .ToListAsync();

        int totalLeaveDays = 0;

        foreach (var leave in leaves)
        {
            var effectiveStart =
                leave.FromDate > startOfMonth
                    ? leave.FromDate
                    : startOfMonth;

            var effectiveEnd =
                leave.ToDate < endOfMonth
                    ? leave.ToDate
                    : endOfMonth;

            totalLeaveDays +=
                effectiveEnd.DayNumber
                - effectiveStart.DayNumber
                + 1;
        }

        return totalLeaveDays;
    }

    public async Task<int>
        GetEmployeeAbsentDaysThisMonthAsync(
            int employeeId)
    {
        var today = DateTime.Today;

        int totalWorkingDays =
            GetWorkingDaysInMonth(
                today.Year,
                today.Month);

        int presentDays =
            await GetEmployeeAttendanceThisMonthAsync(
                employeeId);

        int leaveDays =
            await GetEmployeeLeaveDaysThisMonthAsync(
                employeeId);

        int absentDays =
            totalWorkingDays
            - presentDays
            - leaveDays;

        return Math.Max(absentDays, 0);
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

    private int GetWorkingDaysInMonth(
        int year,
        int month)
    {
        int daysInMonth =
            DateTime.DaysInMonth(year, month);

        int workingDays = 0;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);

            if (
                date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday
            )
            {
                workingDays++;
            }
        }

        return workingDays;
    }

    public async Task<int>
        GetAttendanceTodayAsync(
            int departmentId)
    {
        var today =
            DateOnly.FromDateTime(DateTime.Today);

        return await _context.Attendances
            .Include(a => a.Employee)
            .CountAsync(a =>
                a.AttendanceDate == today
                &&
                a.Employee != null
                &&
                a.Employee.Status == EmployeeStatus.Active
                &&
                a.Employee.DepartmentId == departmentId);
    }

    public async Task<int>
        GetActiveEmployeesAsync(
            int departmentId)
    {
        return await _context.Employees
            .CountAsync(e =>
                e.Status == EmployeeStatus.Active
                &&
                e.DepartmentId == departmentId);
    }

    public async Task<int>
        GetPendingLeaveRequestsAsync(
            int departmentId)
    {
        return await _context.Leaves
            .Include(l => l.Employee)
            .CountAsync(l =>
                l.Status == LeaveStatus.Pending
                &&
                l.Employee != null
                &&
                l.Employee.DepartmentId == departmentId);
    }
}
