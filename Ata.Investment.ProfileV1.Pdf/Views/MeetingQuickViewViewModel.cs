using System;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.ProfileV1.Pdf.Views
{
    public class MeetingQuickViewViewModel
    {
        public KycDocument KycDocument { get; }

        public MeetingQuickViewViewModel(KycDocument kycDocument)
        {
            KycDocument = kycDocument;
        }

        public string FirstName
        {
            get
            {
                int nameSplitIndex = KycDocument.PrimaryClient.Name.LastIndexOf(" ", StringComparison.Ordinal);
                return nameSplitIndex < 0 ? KycDocument.PrimaryClient.Name : KycDocument.PrimaryClient.Name[..nameSplitIndex];
            }
        }

        public string LastName
        {
            get
            {
                int nameSplitIndex = KycDocument.PrimaryClient.Name.LastIndexOf(" ", StringComparison.Ordinal);
                return nameSplitIndex < 0 ? "" : KycDocument.PrimaryClient.Name[nameSplitIndex..];
            }
        }

        public string AccountsForProfile(Profile.Domain.Profile.Profile profile)
        {
            Accounts accounts = profile.Accounts;

            string accountsView = "";
            if (accounts.LIF)
            {
                accountsView += "LIF, ";
            }

            if (accounts.RIF)
            {
                accountsView += "RIF, ";
            }

            if (accounts.LIRA)
            {
                accountsView += "LIRA, ";
            }

            if (accounts.RDSP)
            {
                accountsView += "RDSP, ";
            }

            if (accounts.RESP)
            {
                accountsView += "RESP, ";
            }

            if (accounts.RRSP)
            {
                accountsView += "RRSP, ";
            }

            if (accounts.TFSA)
            {
                accountsView += "TFSA, ";
            }

            if (accounts.NonReg)
            {
                accountsView += "NonReg, ";
            }

            accountsView = accountsView[..^2];

            return accountsView;
        }
    }
}