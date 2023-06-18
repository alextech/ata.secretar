using SharedKernel;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class CreateHouseholdCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Household created";

        public Household Household { get; }

        public CreateHouseholdCommand(Household household)
        {
            Household = household;
        }
    }
}