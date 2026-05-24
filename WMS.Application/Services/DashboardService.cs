using WMS.Application.DTOs.Dashboard;
using WMS.Application.DTOs.Project;
using WMS.Application.Interfaces;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class DashboardService
    : IDashboardService
{
    private readonly IDashboardRepository
        _dashboardRepository;

    private readonly IAnnouncementService
        _announcementService;

    private readonly ICurrentUserService
        _currentUser;

    public DashboardService(
        IDashboardRepository dashboardRepository,
        IAnnouncementService announcementService,
        ICurrentUserService currentUser)
    {
        _dashboardRepository =
            dashboardRepository;

        _announcementService =
            announcementService;

        _currentUser = currentUser;
    }

    public async Task<AdminDashboardDto>
        GetAdminDashboardAsync()
    {
        return new AdminDashboardDto
        {
            TotalEmployees =
                await _dashboardRepository
                    .GetTotalEmployeesAsync(),

            TotalDepartments =
                await _dashboardRepository
                    .GetTotalDepartmentsAsync(),

            EmployeesOnLeaveToday =
                await _dashboardRepository
                    .GetEmployeesOnLeaveTodayAsync(),

            AttendanceToday =
                await _dashboardRepository
                    .GetAttendanceTodayAsync(),

            ActiveProjects =
                await _dashboardRepository
                    .GetActiveProjectsAsync(),

            InactiveEmployees =
                await _dashboardRepository
                    .GetInactiveEmployeesAsync()
        };
    }

    public async Task<ManagerDashboardDto>
        GetManagerDashboardAsync()
    {
        var activeEmployees =
            await _dashboardRepository
                .GetActiveEmployeesAsync();

        var attendanceToday =
            await _dashboardRepository
                .GetAttendanceTodayAsync();

        return new ManagerDashboardDto
        {
            TeamAttendanceToday =
                attendanceToday,

            TeamAbsentToday =
                activeEmployees
                - attendanceToday,

            PendingLeaveRequests =
                await _dashboardRepository
                    .GetPendingLeaveRequestsAsync(),

            ActiveProjectAllocations =
                await _dashboardRepository
                    .GetActiveProjectAllocationsAsync(),

            ActiveProjects =
                await _dashboardRepository
                    .GetActiveProjectsAsync()
        };
    }

    public async Task<EmployeeDashboardDto>
        GetEmployeeDashboardAsync()
    {
        var employeeId =
            _currentUser.EmployeeId
            ?? 0;

        var projects =
            await _dashboardRepository
                .GetEmployeeProjectsAsync(
                    employeeId);

        var announcements =
            await _announcementService
                .GetActiveAnnouncementsAsync();

        return new EmployeeDashboardDto
        {
            PresentDaysThisMonth =
                await _dashboardRepository
                    .GetEmployeeAttendanceThisMonthAsync(
                        employeeId),

            LeaveCountThisMonth =
                await _dashboardRepository
                    .GetEmployeeLeaveCountThisMonthAsync(
                        employeeId),

            Announcements =
                announcements,

            AssignedProjects =
                projects.Select(p =>
                    new ProjectResponseDto
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        ClientId = p.ClientId,
                        Status = p.Status.ToString(),
                        StartDate = p.StartDate,
                        EndDate = p.EndDate
                    }).ToList()
        };
    }
}
