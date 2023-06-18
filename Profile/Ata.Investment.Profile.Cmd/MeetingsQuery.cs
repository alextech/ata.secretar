using System;
using System.Collections.Generic;
using MediatR;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Cmd
{
    public class MeetingsQuery : IRequest<IEnumerable<Meeting>>
    {
        public MeetingsQuery(Guid householdId)
        {
            HouseholdId = householdId;
        }

        public Guid HouseholdId { get; }
    }
}