using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.Profile.Application.Profile
{
    public enum JointPriority
    {
        Source,
        Target
    }

    public static class PopulateMeetingExtension
    {
        public static void PopulateFrom(
            this Meeting target, Meeting source)
        {

            target.XmlKycDocumentSource = source.XmlKycDocumentSource;
            target.Purpose = source.Purpose;
            target.Date = source.Date;
            target.CreatedFor = source.CreatedFor;
        }

    }
}