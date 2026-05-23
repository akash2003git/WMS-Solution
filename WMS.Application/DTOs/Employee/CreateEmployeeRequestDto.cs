namespace WMS.Application.DTOs.Employee;

public class CreateEmployeeRequestDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public char Gender { get; set; }

    public DateOnly DOB { get; set; }

    public DateOnly DOJ { get; set; }

    public int DepartmentId { get; set; }

    public int RoleId { get; set; }
}
