using WMS.Application.DTOs.Auth;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeRequestDto request);
}
