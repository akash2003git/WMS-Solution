namespace WMS.Application.DTOs.Department;

public class CreateDepartmentRequestDto
{
    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
