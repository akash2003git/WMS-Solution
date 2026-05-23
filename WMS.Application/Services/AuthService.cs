using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Auth;
using WMS.Application.Interfaces;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IAuthRepository authRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _authRepository = authRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _authRepository
            .GetByUsernameAsync(request.Username);

        if (user == null)
        {
            throw new UnauthorizedException(
                "Invalid username or password");
        }

        bool isPasswordValid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException(
                "Invalid username or password");
        }

        string token =
            _jwtTokenGenerator.GenerateToken(user);

        await _authRepository
            .UpdateLastLoginAsync(user.UserId);

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role.RoleName,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            RequiresPasswordChange =
                user.MustChangePassword
        };
    }

    public async Task ResetPasswordAsync(
        ResetPasswordDto request)
    {
        var user = await _authRepository
            .GetByUsernameAsync(request.Username);

        if (user == null)
        {
            throw new UnauthorizedException(
                "Invalid credentials");
        }

        bool isCurrentPasswordValid =
            BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash);

        if (!isCurrentPasswordValid)
        {
            throw new UnauthorizedException(
                "Current password is incorrect");
        }

        string hashedPassword =
            BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword);

        user.PasswordHash = hashedPassword;

        user.MustChangePassword = false;

        await _authRepository
            .UpdateUserAsync(user);
    }
}
