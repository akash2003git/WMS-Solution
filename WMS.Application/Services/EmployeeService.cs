using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeRequestDto request)
    {
        bool usernameExists = await _employeeRepository
            .UsernameExistsAsync(request.Email);

        if (usernameExists)
        {
            throw new BusinessRuleException("User already exists");
        }

        string tempPassword = GenerateTemporaryPassword();

        string passwordHash =
            BCrypt.Net.BCrypt.HashPassword(tempPassword);

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            DOB = request.DOB,
            DOJ = request.DOJ,
            DepartmentId = request.DepartmentId,
            RoleId = request.RoleId
        };

        var userLogin = new UserLogin
        {
            Username = request.Email,
            PasswordHash = passwordHash,
            RoleId = request.RoleId,
            MustChangePassword = true
        };

        await _employeeRepository
            .CreateEmployeeAsync(employee, userLogin);

        return new CreateEmployeeResponseDto
        {
            EmployeeId = employee.EmployeeId,
            Username = userLogin.Username,
            TemporaryPassword = tempPassword
        };
    }

    private static string GenerateTemporaryPassword()
    {
        return $"Temp@{Random.Shared.Next(1000, 9999)}";
    }
}
