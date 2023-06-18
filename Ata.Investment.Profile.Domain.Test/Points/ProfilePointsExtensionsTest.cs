using System;
using System.Collections.Generic;
using System.Globalization;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;
// ReSharper disable InconsistentNaming

namespace Ata.Investment.Profile.Domain.Test.Points
{
    public class ProfilePointsExtensionsTest
    {
        [Fact]
        public void SingleProfile_RiskCapacityConservative_Test()
        {
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(45000, 4, ""),
                    FinancialSituation = 5,
                    NetWorth = new NetWorth(25000, 10000, 1000),
                    Knowledge =  new Knowledge(3, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                TimeHorizon = new TimeHorizon(2020) { Range = new SharedKernel.Range(2021, 2021)},

                Goal = 3,
                DecisionMaking = 10,
                Decline = 10,
                LossesOrGains = 10,
                PercentageOfSavings = 2,
                LossVsGainProfile = 3,
                ActionOnLosses = 10,
                HypotheticalProfile = 3,
            };

            Allocation allocation = profile.CreateDecisionTable().SuggestedAllocation;
            Assert.Equal("Conservative Income", allocation.Name);
            Assert.Equal(2, allocation.RiskLevel);
        }

        [Fact]
        public void BreakdownTest()
        {
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1952","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(45000, 4, ""),
                    FinancialSituation = 5,
                    NetWorth = new NetWorth(250000, 10000, 1000),
                    Knowledge =  new Knowledge(3, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                TimeHorizon = new TimeHorizon(2020) { Range = new SharedKernel.Range(2021, 2021)},

                Goal = 3,
                DecisionMaking = 10,
                Decline = 9,
                LossesOrGains = 8,
                PercentageOfSavings = 2,
                LossVsGainProfile = 3,
                ActionOnLosses = 11,
                HypotheticalProfile = 3,
            };

            DecisionBreakdown breakdown = profile.CreateDecisionTable().DecisionBreakdown;
            Assert.Equal(new Dictionary<string, int>
            {
                { "decisionMaking", 10 },
                { "decline", 9 },
                { "actionOnLosses", 11 },
                { "lossesOrGains", 8 },
                { "lossvsgainprofile", 3 },
                { "hypotheticalProfile", 3}
            }, breakdown.RiskAttitude);

            Assert.Equal(1, breakdown.TimeHorizon);
            Assert.Equal(3, breakdown.InvestmentKnowledge);
            Assert.Equal(3, breakdown.InvestmentObjectives);
        }
    }
}