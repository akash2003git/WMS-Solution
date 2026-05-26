using FluentAssertions;
using Moq;
using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Auth;
using WMS.Application.Interfaces;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();

        _jwtTokenGeneratorMock =
            new Mock<IJwtTokenGenerator>();

        _authService = new AuthService(
            _authRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange

        string password = "Admin@123";

        var user = new UserLogin
        {
            UserId = 1,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(password),

            MustChangePassword = false,

            Role = new Role
            {
                RoleName = "Admin"
            }
        };

        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = password
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user))
            .Returns("fake-jwt-token");

        // Act

        var result =
            await _authService.LoginAsync(request);

        // Assert

        result.Should().NotBeNull();

        result.Token.Should()
            .Be("fake-jwt-token");

        result.Username.Should()
            .Be("admin");

        result.Role.Should()
            .Be("Admin");

        _authRepositoryMock.Verify(
            x => x.UpdateLastLoginAsync(user.UserId),
            Times.Once);
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange

        var user = new UserLogin
        {
            UserId = 1,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),

            Role = new Role
            {
                RoleName = "Admin"
            }
        };

        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = "WrongPassword"
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act

        Func<Task> action =
            async () =>
                await _authService.LoginAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<UnauthorizedException>();

        _authRepositoryMock.Verify(
            x => x.UpdateLastLoginAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange

        var request = new LoginRequestDto
        {
            Username = "unknown",
            Password = "Password123"
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((UserLogin?)null);

        // Act

        Func<Task> action =
            async () =>
                await _authService.LoginAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ResetPassword_ValidRequest_UpdatesPassword()
    {
        // Arrange

        string currentPassword = "OldPassword123";

        var user = new UserLogin
        {
            UserId = 1,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(currentPassword),

            MustChangePassword = true
        };

        var request = new ResetPasswordDto
        {
            Username = "admin",
            CurrentPassword = currentPassword,
            NewPassword = "NewPassword123"
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act

        await _authService.ResetPasswordAsync(request);

        // Assert

        BCrypt.Net.BCrypt.Verify(
            request.NewPassword,
            user.PasswordHash)
            .Should()
            .BeTrue();

        user.MustChangePassword
            .Should()
            .BeFalse();

        _authRepositoryMock.Verify(
            x => x.UpdateUserAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task ResetPassword_InvalidCurrentPassword_ThrowsUnauthorizedException()
    {
        // Arrange

        var user = new UserLogin
        {
            UserId = 1,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        };

        var request = new ResetPasswordDto
        {
            Username = "admin",
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123"
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act

        Func<Task> action =
            async () =>
                await _authService.ResetPasswordAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<UnauthorizedException>();

        _authRepositoryMock.Verify(
            x => x.UpdateUserAsync(It.IsAny<UserLogin>()),
            Times.Never);
    }
}


