using SharedKernel;

namespace Ata.Investment.Profile.Domain.KYC
{
    public class Knowledge : ValueObject<Knowledge>
    {
        public int Level { get; set; }
        public string Notes { get; set; }

        public Knowledge(int level, string notes)
        {
            Level = level;
            Notes = notes;
        }
    }
}