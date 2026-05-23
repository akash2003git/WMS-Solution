using FluentValidation;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Validators.Employee;

public class UpdateEmployeeRequestValidator
    : AbstractValidator<UpdateEmployeeRequestDto>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(80);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(15);

        RuleFor(x => x.Gender)
            .Must(g => g == 'M' || g == 'F' || g == 'O')
            .WithMessage("Gender must be M, F, or O");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department is required");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("Role is required");

        RuleFor(x => x.DOB)
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Employee must be at least 18 years old");

        RuleFor(x => x)
            .Must(x => x.DOJ >= x.DOB)
            .WithMessage("DOJ cannot be before DOB");
    }

    private static bool BeAtLeast18YearsOld(DateOnly dob)
    {
        int age = DateTime.Today.Year - dob.Year;

        if (dob > DateOnly.FromDateTime(
                DateTime.Today.AddYears(-age)))
        {
            age--;
        }

        return age >= 18;
    }
}
