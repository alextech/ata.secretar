using System.Collections.Generic;
using MediatR;

namespace Ata.Investment.Profile.Cmd.Advisors
{
    public class AllAdvisorsQuery : IRequest<IEnumerable<Domain.Advisor>>
    {

    }
}