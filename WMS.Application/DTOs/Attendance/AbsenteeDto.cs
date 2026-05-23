namespace WMS.Application.DTOs.Attendance;

public class AbsenteeDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
