namespace WMS.Application.DTOs.Project;

public class ProjectAllocationDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateOnly AssignedOn { get; set; }

    public bool Status { get; set; }
}
