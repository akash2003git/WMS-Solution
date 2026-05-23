using WMS.Application.DTOs.Department;

namespace WMS.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentResponseDto>> GetDepartmentsAsync();

    Task<DepartmentResponseDto> GetDepartmentByIdAsync(int departmentId);

    Task<List<DepartmentEmployeeDto>> GetDepartmentEmployeesAsync(
        int departmentId);

    Task CreateDepartmentAsync(
        CreateDepartmentRequestDto request);

    Task UpdateDepartmentAsync(
        int departmentId,
        UpdateDepartmentRequestDto request);

    Task DeleteDepartmentAsync(int departmentId);
}
