namespace WMS.Application.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalEmployees { get; set; }

    public int TotalDepartments { get; set; }

    public int EmployeesOnLeaveToday { get; set; }

    public int AttendanceToday { get; set; }

    public int ActiveProjects { get; set; }

    public int InactiveEmployees { get; set; }
}
