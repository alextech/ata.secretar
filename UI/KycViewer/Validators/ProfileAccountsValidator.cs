using FluentValidation;
using Ata.Investment.Profile.Domain.Profile;

namespace KycViewer.Validators
{
    public class ProfileAccountsValidator : AbstractValidator<Profile>
    {
        public ProfileAccountsValidator()
        {
            RuleFor(p => p.Accounts)
                .Must(a => a.NonReg || a.LIRA || a.RDSP || a.RESP || a.RRSP || a.TFSA || a.LIF || a.RIF)
                .WithMessage("Need to select at least one profile.");
        }


    }
}