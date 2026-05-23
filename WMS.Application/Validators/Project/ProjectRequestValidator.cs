using FluentValidation;
using WMS.Application.DTOs.Project;

namespace WMS.Application.Validators.Project;

public class CreateProjectRequestValidator
    : AbstractValidator<CreateProjectRequestDto>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.ProjectName).NotEmpty().MaximumLength(100);

        RuleFor(x => x)
          .Must(x => !x.StartDate.HasValue
                || !x.EndDate.HasValue
                || x.StartDate <= x.EndDate)
            .WithMessage("StartDate cannot be after EndDate");
    }
}
