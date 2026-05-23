using FluentValidation;
using WMS.Application.DTOs.Project;

namespace WMS.Application.Validators.Project;

public class UpdateProjectRequestValidator
    : AbstractValidator<UpdateProjectRequestDto>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x)
            .Must(x =>
                !x.StartDate.HasValue
                ||
                !x.EndDate.HasValue
                ||
                x.StartDate <= x.EndDate)
            .WithMessage(
                "StartDate cannot be after EndDate");
    }
}
