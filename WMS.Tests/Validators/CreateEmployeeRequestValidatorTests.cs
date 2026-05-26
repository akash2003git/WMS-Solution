using FluentValidation.TestHelper;
using WMS.Application.DTOs.Employee;
using WMS.Application.Validators.Employee;

namespace WMS.Tests.Validators;

public class CreateEmployeeRequestValidatorTests
{
    private readonly CreateEmployeeRequestValidator _validator;

    public CreateEmployeeRequestValidatorTests()
    {
        _validator = new CreateEmployeeRequestValidator();
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveValidationError()
    {
        // Arrange

        var model = new CreateEmployeeRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 1
        };

        // Act

        var result = _validator.TestValidate(model);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_EmptyFirstName_ShouldHaveValidationError()
    {
        // Arrange

        var model = new CreateEmployeeRequestDto
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 1
        };

        // Act

        var result = _validator.TestValidate(model);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}
