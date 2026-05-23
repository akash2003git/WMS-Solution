using WMS.Application.DTOs.Employee;

namespace WMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<CreateEmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeRequestDto request);
}
