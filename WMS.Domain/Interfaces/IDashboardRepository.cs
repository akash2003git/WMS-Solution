using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<int> GetTotalEmployeesAsync();

    Task<int> GetTotalDepartmentsAsync();

    Task<int> GetEmployeesOnLeaveTodayAsync();

    Task<int> GetAttendanceTodayAsync();

    Task<int> GetActiveProjectsAsync();

    Task<int> GetInactiveEmployeesAsync();

    Task<int> GetActiveEmployeesAsync();

    Task<int> GetPendingLeaveRequestsAsync();

    Task<int> GetActiveProjectAllocationsAsync();

    Task<int> GetEmployeeAttendanceThisMonthAsync(
        int employeeId);

    Task<int> GetEmployeeLeaveCountThisMonthAsync(
        int employeeId);

    Task<List<Project>>
        GetEmployeeProjectsAsync(
            int employeeId);
}
