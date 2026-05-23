using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetDepartmentsAsync();

    Task<Department?> GetByIdAsync(int departmentId);

    Task<bool> DepartmentNameExistsAsync(string departmentName);

    Task<bool> DepartmentNameExistsAsync(
        string departmentName,
        int excludeDepartmentId);

    Task<bool> HasEmployeesAsync(int departmentId);

    Task CreateDepartmentAsync(Department department);

    Task UpdateDepartmentAsync(Department department);

    Task DeleteDepartmentAsync(Department department);
}
