using FluentValidation;
using Ata.Investment.Profile.Domain.Profile;

namespace KycViewer.Validators
{
    public class ExpectationsValidator : AbstractValidator<Profile>
    {
        public ExpectationsValidator()
        {
            RuleFor(p => p.TimeHorizon.WithdrawTime)
                .GreaterThan(-1)
                .WithMessage("Profile should have time horizon selected.");

            RuleFor(p => p.Goal)
                .GreaterThan(-1)
                .WithMessage("Should specify profile goal.");

            RuleFor(p => p.PercentageOfSavings)
                .GreaterThan(-1)
                .WithMessage("Should specify what percentage of savings the profile makes up.");

            RuleFor(p => p.DecisionMaking)
                .GreaterThan(-1)
                .WithMessage("Should specify how make financial decisions for this portfolio.");

            RuleFor(p => p.Decline)
                .GreaterThan(-1)
                .WithMessage("Should specify amount of decline willing to tolerate within 12 months.");

            RuleFor(p => p.LossesOrGains)
                .GreaterThan(-1)
                .WithMessage("Should specify if concerned about losses or gains.");

            RuleFor(p => p.ActionOnLosses)
                .GreaterThan(-1)
                .WithMessage("Should specify action taken when losses occur.");

            RuleFor(p => p.LossVsGainProfile)
                .GreaterThan(-1)
                .WithMessage("Should specify which loss vs gains profile comfortable with.");

            RuleFor(p => p.HypotheticalProfile)
                .GreaterThan(-1)
                .WithMessage("Should specify risk level portfolio comfortable with.");
        }
    }
}