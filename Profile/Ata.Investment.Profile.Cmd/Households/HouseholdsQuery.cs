using System.Collections.Generic;
using MediatR;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd.Households
{
    #nullable enable
    public class HouseholdsQuery : IRequest<IEnumerable<Household>>
    {
        public string? ClientName { get; init; }
        public bool? IsActive { get; init; }
        public bool? IsArchived { get; init; }
    }
    #nullable restore
}