using FluentValidation;
using WMS.Application.DTOs.Department;

namespace WMS.Application.Validators.Department;

public class UpdateDepartmentRequestValidator
    : AbstractValidator<UpdateDepartmentRequestDto>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(x => x.DepartmentName)
            .NotEmpty()
            .WithMessage("Department name is required")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(255);
    }
}
