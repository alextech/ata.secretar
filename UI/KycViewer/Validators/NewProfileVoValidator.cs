using System.Diagnostics;
using System.Linq;
using FluentValidation;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;

namespace KycViewer.Validators
{
    public class NewProfileVoValidator : AbstractValidator<NewProfileVO>
    {

        public NewProfileVoValidator(KycDocument kycDocument)
        {
            RuleFor(np => np.Name)
                .NotNull()
                .NotEmpty()
                .Must(IsNameUnique)
                .WithMessage("Must give profile a unique name.");

            RuleFor(np => np.Accounts)
                .Must(a => a.NonReg || a.LIRA || a.RDSP || a.RESP || a.RRSP || a.TFSA || a.LIF || a.RIF)
                .WithMessage("Need to select at least one profile.");

            RuleFor(np => np.TimeHorizon.Range.Max - np.TimeHorizon.Range.Min)
                .GreaterThan(-1)
                .WithMessage("Need to select time horizon");
        }

        private static bool IsNameUnique(NewProfileVO newProfileVo, string profileName)
        {

            bool exists_p;

            if (!newProfileVo.IsJoint)
            {
                exists_p = newProfileVo.Primary.Profiles.All(p => p.Name != profileName);
            }
            else
            {
                Debug.Assert(newProfileVo.KycDocument != null, "newProfileVo.KycDocument != null");
                exists_p = newProfileVo.KycDocument.JointProfiles.All(p => p.Name != profileName);
            }

            return exists_p;
        }
    }
}