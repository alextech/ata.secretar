using System.Collections.Generic;
using System.Data;
using Ata.Investment.Profile.Domain.Points.Tree;

namespace Ata.Investment.Profile.Domain.Points
{
    public class DecisionBreakdown
    {
        public Profile.Profile Profile { get; set; }
        public Dictionary<string, int> RiskAttitude { get; internal set; } = new Dictionary<string, int>();
        public IExpression RiskCapacity { get; internal set; }
        public int InvestmentObjectives { get; set; }
        public int InvestmentKnowledge { get; set; }
        public int TimeHorizon { get; set; }
    }
}