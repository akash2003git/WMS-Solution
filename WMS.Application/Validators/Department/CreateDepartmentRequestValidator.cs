using FluentValidation;
using WMS.Application.DTOs.Department;

namespace WMS.Application.Validators.Department;

public class CreateDepartmentRequestValidator
    : AbstractValidator<CreateDepartmentRequestDto>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.DepartmentName)
            .NotEmpty()
            .WithMessage("Department name is required")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(255);
    }
}
