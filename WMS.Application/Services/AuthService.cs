using WMS.Application.DTOs.Auth;
using WMS.Application.Interfaces;
using WMS.Domain.Interfaces;
using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Employee;
using WMS.Domain.Entities;

namespace WMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IAuthRepository authRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _authRepository = authRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    private static string GenerateTemporaryPassword()
    {
        return $"Temp@{Random.Shared.Next(1000, 9999)}";
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            throw new UnauthorizedException(
                "Invalid username or password");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        await _authRepository.UpdateLastLoginAsync(user.UserId);

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role.RoleName,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeRequestDto request)
    {
        bool usernameExists = await _authRepository
            .UsernameExistsAsync(request.Email);

        if (usernameExists)
        {
            throw new BusinessRuleException("User already exists");
        }

        string tempPassword = GenerateTemporaryPassword();

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

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

        await _authRepository.CreateEmployeeAsync(employee, userLogin);

        return new CreateEmployeeResponseDto
        {
            EmployeeId = employee.EmployeeId,
            Username = userLogin.Username,
            TemporaryPassword = tempPassword
        };
    }
}
