using System;
using MediatR;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd
{
    public class HouseholdByClientIdQuery : IRequest<Household>
    {
        public Guid ClientId { get; }

        public HouseholdByClientIdQuery(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}