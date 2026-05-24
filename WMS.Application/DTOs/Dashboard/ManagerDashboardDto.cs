namespace WMS.Application.DTOs.Dashboard;

public class ManagerDashboardDto
{
    public int TeamAttendanceToday { get; set; }

    public int TeamAbsentToday { get; set; }

    public int PendingLeaveRequests { get; set; }

    public int ActiveProjectAllocations { get; set; }

    public int ActiveProjects { get; set; }
}
