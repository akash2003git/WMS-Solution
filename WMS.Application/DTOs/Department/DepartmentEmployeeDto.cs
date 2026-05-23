namespace WMS.Application.DTOs.Department;

public class DepartmentEmployeeDto
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
