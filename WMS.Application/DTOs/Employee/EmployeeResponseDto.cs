namespace WMS.Application.DTOs.Employee;

public class EmployeeResponseDto
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public char Gender { get; set; }

    public DateOnly DOB { get; set; }

    public DateOnly DOJ { get; set; }

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
