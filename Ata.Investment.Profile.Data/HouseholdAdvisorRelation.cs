using System;
using System.Collections.Generic;

namespace Ata.Investment.Profile.Data
{
    public class HouseholdAdvisorRelation
    {
        public Guid HouseholdId { get; set; }
        public List<Guid> AdvisorId { get; set; }
    }
}