using FluentValidation;
using WMS.Application.DTOs.Leave;

namespace WMS.Application.Validators.Leave;

public class ApplyLeaveRequestValidator
    : AbstractValidator<ApplyLeaveRequestDto>
{
    public ApplyLeaveRequestValidator()
    {
        RuleFor(x => x.LeaveType)
            .NotEmpty();

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage(
                "FromDate cannot be after ToDate");

        RuleFor(x => x.Reason)
            .MaximumLength(255);
    }
}
