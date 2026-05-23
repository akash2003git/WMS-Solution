namespace WMS.Application.DTOs.Report;

public class ProjectAllocationReportDto
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateOnly AssignedOn { get; set; }
}
