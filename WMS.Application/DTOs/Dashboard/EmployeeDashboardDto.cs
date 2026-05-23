using WMS.Application.DTOs.Announcement;
using WMS.Application.DTOs.Project;

namespace WMS.Application.DTOs.Dashboard;

public class EmployeeDashboardDto
{
    public int PresentDaysThisMonth { get; set; }

    public int LeaveCountThisMonth { get; set; }

    public List<AnnouncementResponseDto>
        Announcements
    { get; set; } = [];

    public List<ProjectResponseDto>
        AssignedProjects
    { get; set; } = [];
}
