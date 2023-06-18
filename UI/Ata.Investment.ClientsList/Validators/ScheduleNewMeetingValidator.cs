using System;
using FluentValidation;
using Ata.Investment.ClientsList.ViewModels;

namespace Ata.Investment.ClientsList.Validators;

public class ScheduleNewMeetingValidator : AbstractValidator<ScheduleNewMeetingVM>
{
    public ScheduleNewMeetingValidator()
    {
        RuleFor(ms => ms.AdvisorId)
            .NotEmpty()
            .WithMessage("No advisor selected.");
        RuleFor(ms => ms.DateTime)
            .Must(date => date > DateTimeOffset.Now)
            .WithMessage("Meeting date should be set after today.");
    }
}