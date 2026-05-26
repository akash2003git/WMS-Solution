using FluentValidation.TestHelper;
using WMS.Application.DTOs.Auth;
using WMS.Application.Validators.Auth;

namespace WMS.Tests.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTests()
    {
        _validator = new LoginRequestValidator();
    }

    [Fact]
    public void Validate_EmptyUsername_ShouldHaveValidationError()
    {
        // Arrange

        var model = new LoginRequestDto
        {
            Username = "",
            Password = "Password123"
        };

        // Act

        var result = _validator.TestValidate(model);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldHaveValidationError()
    {
        // Arrange

        var model = new LoginRequestDto
        {
            Username = "admin",
            Password = ""
        };

        // Act

        var result = _validator.TestValidate(model);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
