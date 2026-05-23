using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<bool> UsernameExistsAsync(string username);

    Task CreateEmployeeAsync(Employee employee, UserLogin userLogin);

    Task<Employee?> GetByIdAsync(int employeeId);

    Task<Employee?> GetEmployeeByEmailAsync(string email);

    Task<List<Employee>> GetEmployeesAsync();

    Task<bool> DepartmentExistsAsync(int departmentId);

    Task<bool> RoleExistsAsync(int roleId);

    Task UpdateEmployeeAsync(Employee employee);

    Task SaveChangesAsync();
}
