namespace WMS.Application.DTOs.Department;

public class UpdateDepartmentRequestDto
{
    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
