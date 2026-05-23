using FluentValidation;
using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Validators.Announcement;

public class CreateAnnouncementRequestValidator
    : AbstractValidator<CreateAnnouncementRequestDto>
{
    public CreateAnnouncementRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty();
    }
}
