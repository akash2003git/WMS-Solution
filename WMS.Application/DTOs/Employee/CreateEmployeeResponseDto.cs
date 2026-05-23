namespace WMS.Application.DTOs.Employee;

public class CreateEmployeeResponseDto
{
    public int EmployeeId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;
}
