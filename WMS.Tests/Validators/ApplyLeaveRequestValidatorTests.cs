using FluentValidation.TestHelper;
using WMS.Application.DTOs.Leave;
using WMS.Application.Validators.Leave;

namespace WMS.Tests.Validators;

public class ApplyLeaveRequestValidatorTests
{
    private readonly ApplyLeaveRequestValidator _validator;

    public ApplyLeaveRequestValidatorTests()
    {
        _validator = new ApplyLeaveRequestValidator();
    }

    [Fact]
    public void Validate_FromDateAfterToDate_ShouldHaveValidationError()
    {
        // Arrange

        var model = new ApplyLeaveRequestDto
        {
            LeaveType = "Sick",
            FromDate = new DateOnly(2026, 5, 10),
            ToDate = new DateOnly(2026, 5, 1),
            Reason = "Medical leave"
        };

        // Act

        var result = _validator.TestValidate(model);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FromDate);
    }
}
