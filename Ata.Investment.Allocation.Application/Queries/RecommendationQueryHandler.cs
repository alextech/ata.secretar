using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application.Queries
{
    public class RecommendationQueryHandler : IRequestHandler<RecommendationQuery, IEnumerable<Recommendation>>
    {
        private readonly AllocationContext _allocationContext;

        public RecommendationQueryHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<IEnumerable<Recommendation>> Handle(RecommendationQuery query, CancellationToken cancellationToken)
        {
            AllocationVersion allocationVersion = await (
                from a in _allocationContext.AllocationVersions.AsNoTracking()
                    .Include(allocation => allocation.Options)
                    .ThenInclude(alo => alo.Option)
                    .TagWith("Allocation by points, specific version.")
                where a.Version == query.Version &&
                      a.RiskLevel == query.RiskLevel
                select a
            ).SingleAsync(cancellationToken: cancellationToken);

            IEnumerable<Recommendation> recommendations =
                from a in allocationVersion.Options
                select new Recommendation(
                    allocationVersion.Name,
                    a.Option.Name,
                    a.CompositionParts.ToDictionary(c => c.FundCode, c => c.Percent),
                    a.Id
                );

            return recommendations;
        }

        public void Dispose()
        {
            _allocationContext.Dispose();
        }
    }
}