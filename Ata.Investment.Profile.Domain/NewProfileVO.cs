using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.Profile.Domain
{
    public class NewProfileVO
    {
        public PClient Primary { get; }
        public PClient? Joint { get; }
        public KycDocument KycDocument { get; }

        public NewProfileVO(PClient primary, KycDocument kycDocument)
        {
            Primary = primary;
            KycDocument = kycDocument;
        }

        public NewProfileVO(PClient primary, PClient joint, KycDocument kycDocument)
        {
            Primary = primary;
            Joint = joint;
            KycDocument = kycDocument;
        }

        public bool IsJoint => Joint != null;

        public Accounts Accounts { get; set; } = new Accounts();

        public string Name { get; set; } = "";
        public string Icon { get; set; } = "dollar";
        public TimeHorizon TimeHorizon { get; set; }

        public string ProfileFor => $"New profile for {Primary.Name}{(IsJoint ? $" and {Joint?.Name}" : "")}";

        public int TimeOrigin => KycDocument.Date.Year;
    }
}