using System.Data;
using System.Linq;
using SharedKernel;
using Ata.Investment.Profile.Domain.Points;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Points
{
    public class RiskTableTest
    {
        [Fact]
        public void RiskSelection_TimeHorizon_Test()
        {
            RiskTable riskTable = new RiskTable();
            int risk = (
                from row in riskTable.AsEnumerable()
                where row.Field<Range>("TimeHorizon").Min <= 4 &&
                      row.Field<Range>("TimeHorizon").Max >= 4
                select row.Field<int>("RiskLevel")
            ).Single();

            Assert.Equal(4, risk);
        }
    }
}