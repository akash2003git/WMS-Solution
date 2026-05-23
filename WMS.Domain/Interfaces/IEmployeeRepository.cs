using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task CreateEmployeeAsync(Employee employee, UserLogin userLogin);
}
