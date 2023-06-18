using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using Ata.Investment.ClientsList.ViewModels;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Domain.KYC;

namespace Ata.Investment.ClientsList.Validators
{
    public class NewClientValidator : AbstractValidator<ClientVM>
    {
        public NewClientValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Client should have a unique name.");

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email is invalid format.");

            RuleFor(c => c.DateOfBirth)
                .Must(
                    date =>
                    {
                        int age = PClient.AgeFromBirthdate(date);
                        return age is > 18 and < 90;
                    })
                .WithMessage("Date of birth format: year-month-day or day-month-year and be between 18 and 90 years old");
        }
    }
}