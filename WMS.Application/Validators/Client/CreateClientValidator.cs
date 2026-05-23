using FluentValidation;
using WMS.Application.DTOs.Client;

namespace WMS.Application.Validators.Client;

public class CreateClientRequestValidator
    : AbstractValidator<CreateClientRequestDto>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ClientPhoneNumber)
            .MaximumLength(15);

        RuleFor(x => x.ClientLocation)
            .MaximumLength(20);
    }
}
