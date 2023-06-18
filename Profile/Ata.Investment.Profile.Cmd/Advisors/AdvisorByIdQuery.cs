using System;
using MediatR;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Cmd.Advisors
{
    public class AdvisorByIdQuery : IRequest<Advisor>
    {
        public Guid AdvisorId { get; }

        public AdvisorByIdQuery(Guid advisorId)
        {
            AdvisorId = advisorId;
        }
    }
}