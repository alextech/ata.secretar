using System.Collections.Generic;
using MediatR;
using SharedKernel;

namespace Ata.Investment.Allocation.Cmd
{
    public class RecommendationQuery : IRequest<IEnumerable<Recommendation>>
    {
        public int RiskLevel { get; }
        public int Version { get; }
        public RecommendationQuery(int riskLevel, int version)
        {
            RiskLevel = riskLevel;
            Version = version;
        }
    }
}