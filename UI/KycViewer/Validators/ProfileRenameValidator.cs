using System.Diagnostics;
using System.Linq;
using FluentValidation;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Domain.Profile;

namespace KycViewer.Validators
{
    public class ProfileRenameValidator : AbstractValidator<ProfileRenameVO>
    {
        public ProfileRenameValidator()
        {
            RuleFor(np => np.Name)
                .NotNull()
                .NotEmpty()
                .Must(IsNameUnique)
                .WithMessage("Must give profile a unique name.");

            // RuleFor(np => np.Icon)
            //     .NotNull()
            //     .NotEmpty()
            //     .WithMessage("Must give profile icon");
        }

        private static bool IsNameUnique(ProfileRenameVO profileRenameVo, string profileName)
        {
            Profile profile = profileRenameVo.Profile;

            // if same name, do not need to deny it.
            if (profile.Name == profileName)
            {
                return true;
            }

            bool profileExists = profile.PrimaryClient.Profiles.All(p => p.Name != profileName);

            if (!profile.IsJoint) return profileExists;
            {
                Debug.Assert(profile.JointClient != null, "newProfileVo.Joint != null");
                bool jointExists = profile.JointClient.Profiles.All(p => p.Name != profileName);
                return profileExists && jointExists;
            }
        }
    }
}