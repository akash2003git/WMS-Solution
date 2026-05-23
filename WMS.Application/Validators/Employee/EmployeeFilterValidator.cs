using FluentValidation;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Validators.Employee;

public class EmployeeFilterValidator
    : AbstractValidator<EmployeeFilterDto>
{
    public EmployeeFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .When(x => !string.IsNullOrWhiteSpace(x.SortDirection))
            .WithMessage("SortDirection must be asc or desc");
    }

    private static bool BeValidSortDirection(string? direction)
    {
        if (string.IsNullOrWhiteSpace(direction))
        {
            return true;
        }

        direction = direction.ToLower();

        return direction == "asc" || direction == "desc";
    }
}
