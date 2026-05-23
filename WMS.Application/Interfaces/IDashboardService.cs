using WMS.Application.DTOs.Dashboard;

namespace WMS.Application.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto>
        GetAdminDashboardAsync();

    Task<ManagerDashboardDto>
        GetManagerDashboardAsync();

    Task<EmployeeDashboardDto>
        GetEmployeeDashboardAsync();
}
