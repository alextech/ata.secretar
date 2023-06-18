using FluentValidation;
using Ata.Investment.Profile.Domain.KYC;

namespace KycViewer.Validators
{
    public class PClientValidator : AbstractValidator<PClient>
    {
        public PClientValidator()
        {
            RuleFor(c => c.Income.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Cannot have negative income");

            RuleFor(c => c.Knowledge.Level)
                .GreaterThan(0)
                .WithMessage("Client should have knowledge level selected above Little to None.");

            RuleFor(c => c.Income.Stability)
                .GreaterThan(-1)
                .WithMessage("Income source stability should be selected.");

            RuleFor(c => c.FinancialSituation)
                .GreaterThan(-1)
                .WithMessage("Financial situation should be selected.");

        }
    }
}