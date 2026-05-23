using WMS.Application.Common.Models;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<CreateEmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeRequestDto request);

    Task<EmployeeResponseDto> GetEmployeeByIdAsync(int employeeId);

    Task<PagedResponse<EmployeeResponseDto>> GetEmployeesAsync(
        EmployeeFilterDto filter);

    Task UpdateEmployeeAsync(
        int employeeId,
        UpdateEmployeeRequestDto request);

    Task DeleteEmployeeAsync(int employeeId);
}
