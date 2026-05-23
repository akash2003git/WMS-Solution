using FluentValidation;
using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Validators.Announcement;

public class UpdateAnnouncementRequestValidator
    : AbstractValidator<UpdateAnnouncementRequestDto>
{
    public UpdateAnnouncementRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty();
    }
}
