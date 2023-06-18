using System;
using System.Collections.Generic;
using System.Linq;
using Ata.Investment.Profile.Domain.KYC;

namespace Ata.Investment.Profile.Domain
{
    public static class KycDocumentExtensions
    {
        public static PClient GetClientById(this KycDocument kycDocument, Guid clientId)
        {
            if (kycDocument.PrimaryClient.Guid == clientId)
            {
                return kycDocument.PrimaryClient;
            }

            if (kycDocument.IsJoint == false)
            {
                throw new NonExistentClientRequestedException(clientId);
            }

            if (kycDocument.JointClient.Guid == clientId)
            {
                return kycDocument.JointClient;
            }

            throw new NonExistentClientRequestedException(clientId);
        }

        public static Domain.Profile.Profile GetProfileById(this KycDocument kycDocument, Guid profileId)
        {
            IEnumerable<Domain.Profile.Profile> profiles = kycDocument.PrimaryClient.Profiles;
            if (kycDocument.IsJoint)
            {
                profiles = profiles.Concat(kycDocument.JointClient.Profiles)
                    .Concat(kycDocument.JointProfiles);
            }


            Domain.Profile.Profile profile = profiles.SingleOrDefault(p => p.Guid == profileId);
            return profile;
        }
    }

    public class NonExistentClientRequestedException : ArgumentOutOfRangeException
    {
        public NonExistentClientRequestedException(Guid clientId) : base("Tried to query non existing client: "+clientId)
        {

        }
    }

}