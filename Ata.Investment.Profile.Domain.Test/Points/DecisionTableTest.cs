using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Points
{
    public class DecisionTableTest
    {
        [Fact]
        public void SelectMostConservativeAllocationTest()
        {
            // ==== SETUP ====
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(45000, 4, ""),
                    FinancialSituation = 5,
                    NetWorth = new NetWorth(25000, 10000, 1000),
                    Knowledge =  new Knowledge(2, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                // TimeHorizon = 10,

                Goal = 3,
                DecisionMaking = 10,
                Decline = 10,
                LossesOrGains = 10,
                PercentageOfSavings = 2,
                LossVsGainProfile = 3,
                ActionOnLosses = 10,
                HypotheticalProfile = 3,
            };

            DecisionTable decisionTable = new DecisionTable(profile);

            decisionTable.Rows[1]["TimeHorizon"] = true;
            decisionTable.Rows[2]["InvestmentKnowledge"] = true;
            decisionTable.Rows[2]["InvestmentObjectives"] = true;
            decisionTable.Rows[4]["RiskCapacity"] = true;
            decisionTable.Rows[4]["RiskAttitude"] = true;
            
            // offset by 1 from row index to business numbers
            Assert.Equal(2, decisionTable.SuggestedAllocation.RiskLevel);
            Assert.Equal("Conservative Income", decisionTable.SuggestedAllocation.Name);
        }
        
        [Fact]
        public void TimeHorizonRangeTest()
        {
            // ==== SETUP ====
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(45000, 4, ""),
                    FinancialSituation = 5,
                    NetWorth = new NetWorth(25000, 10000, 1000),
                    Knowledge =  new Knowledge(2, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                // TimeHorizon = 1,

                Goal = 3,
                DecisionMaking = 10,
                Decline = 10,
                LossesOrGains = 10,
                PercentageOfSavings = 2,
                LossVsGainProfile = 3,
                ActionOnLosses = 10,
                HypotheticalProfile = 3,
            };

            DecisionTable decisionTable = new DecisionTable(profile);

            decisionTable.Rows[0]["TimeHorizon"] = true;
            decisionTable.Rows[3]["InvestmentKnowledge"] = true;
            decisionTable.Rows[4]["InvestmentObjectives"] = true;
            decisionTable.Rows[3]["RiskCapacity"] = true;
            decisionTable.Rows[4]["RiskAttitude"] = true;
            
            // offset by 1 from row index to business numbers
            Assert.Equal(1, decisionTable.SuggestedAllocation.RiskLevel);
            Assert.Equal("Safety", decisionTable.SuggestedAllocation.Name);
        }

        [Fact]
        public void PivotTest()
        {
            // ==== SETUP ====
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(45000, 4, ""),
                    FinancialSituation = 5,
                    NetWorth = new NetWorth(25000, 10000, 1000),
                    Knowledge =  new Knowledge(2, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                // TimeHorizon = 1,

                Goal = 3,
                DecisionMaking = 10,
                Decline = 10,
                LossesOrGains = 10,
                PercentageOfSavings = 2,
                LossVsGainProfile = 3,
                ActionOnLosses = 10,
                HypotheticalProfile = 3,
            };

            DecisionTable decisionTable = new DecisionTable(profile);

            decisionTable.Rows[3]["TimeHorizon"] = true;
            decisionTable.Rows[2]["InvestmentKnowledge"] = true;
            decisionTable.Rows[4]["InvestmentObjectives"] = true;
            decisionTable.Rows[2]["RiskCapacity"] = true;
            decisionTable.Rows[3]["RiskAttitude"] = true;
            
            // ==== ACT ====
            DataTable pivot = decisionTable.Pivot;

            // ==== VERIFY ====
            // Time Horizon
            Assert.Equal(false, pivot.Rows[0].ItemArray[0]);
            Assert.Equal(false, pivot.Rows[0].ItemArray[1]);
            Assert.Equal(false, pivot.Rows[0].ItemArray[2]);
            Assert.Equal(true,  pivot.Rows[0].ItemArray[3]);
            Assert.Equal(false, pivot.Rows[0].ItemArray[4]);
            
            // Investment Knowledge
            Assert.Equal(false, pivot.Rows[1].ItemArray[0]);
            Assert.Equal(false, pivot.Rows[1].ItemArray[1]);
            Assert.Equal(true,  pivot.Rows[1].ItemArray[2]);
            Assert.Equal(false, pivot.Rows[1].ItemArray[3]);
            Assert.Equal(false, pivot.Rows[1].ItemArray[4]);
            
            // Investment Objectives
            Assert.Equal(false, pivot.Rows[2].ItemArray[0]);
            Assert.Equal(false, pivot.Rows[2].ItemArray[1]);
            Assert.Equal(false, pivot.Rows[2].ItemArray[2]);
            Assert.Equal(false, pivot.Rows[2].ItemArray[3]);
            Assert.Equal(true,  pivot.Rows[2].ItemArray[4]);
            
            // Risk Capacity
            Assert.Equal(false, pivot.Rows[3].ItemArray[0]);
            Assert.Equal(false, pivot.Rows[3].ItemArray[1]);
            Assert.Equal(true,  pivot.Rows[3].ItemArray[2]);
            Assert.Equal(false, pivot.Rows[3].ItemArray[3]);
            Assert.Equal(false, pivot.Rows[3].ItemArray[4]);
            
            // Risk Attitude
            Assert.Equal(false, pivot.Rows[4].ItemArray[0]);
            Assert.Equal(false, pivot.Rows[4].ItemArray[1]);
            Assert.Equal(false, pivot.Rows[4].ItemArray[2]);
            Assert.Equal(true,  pivot.Rows[4].ItemArray[3]);
            Assert.Equal(false, pivot.Rows[4].ItemArray[4]);
        }

        [Theory]
        [MemberData(nameof(SuggestedObjectivesData))]
        public void SuggestedObjectivesTest(
            int riskLevel,
            int aggressiveGrowth,
            int growth,
            int balanced,
            int income,
            int cashReserve
        )
        {
            // ==== SETUP ====
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(0, 0, ""),
                    FinancialSituation = 0,
                    NetWorth = new NetWorth(0, 0, 0),
                    Knowledge =  new Knowledge(0, "")
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
            };

            DecisionTable decisionTable = new DecisionTable(profile);

            decisionTable.Rows[riskLevel]["TimeHorizon"] = true;
            decisionTable.Rows[riskLevel]["InvestmentKnowledge"] = true;
            decisionTable.Rows[riskLevel]["InvestmentObjectives"] = true;
            decisionTable.Rows[riskLevel]["RiskCapacity"] = true;
            decisionTable.Rows[riskLevel]["RiskAttitude"] = true;


            Objectives suggestedObjectives = decisionTable.SuggestedObjectives;

            Assert.Equal(aggressiveGrowth, suggestedObjectives.AggressiveGrowth);
            Assert.Equal(growth, suggestedObjectives.Growth);
            Assert.Equal(balanced, suggestedObjectives.Balanced);
            Assert.Equal(income, suggestedObjectives.Income);
            Assert.Equal(cashReserve, suggestedObjectives.CashReserve);
        }

        [Theory]
        [MemberData(nameof(SuggestedRiskToleranceData))]
        public void SuggestedRiskToleranceTest(
            int riskLevel,
            int high,
            int mediumHigh,
            int medium,
            int lowMedium,
            int low
        )
        {
            // ==== SETUP ====
            PClient pclient =
                new PClient(new Client("p_client", "primary@points.test", DateTimeOffset.ParseExact("14/04/1989","dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    Income = new Income(0, 0, ""),
                    FinancialSituation = 10,
                    NetWorth = new NetWorth(0, 0, 0),
                    Knowledge =  new Knowledge(0, ""),
                };
            Domain.Profile.Profile profile = new Domain.Profile.Profile(pclient)
            {
                TimeHorizon = new TimeHorizon(2020) { Range = new SharedKernel.Range(2020, 2020)},
            };

            DecisionTable decisionTable = new DecisionTable(profile);

            decisionTable.Rows[riskLevel]["TimeHorizon"] = true;
            decisionTable.Rows[riskLevel]["InvestmentKnowledge"] = true;
            decisionTable.Rows[riskLevel]["InvestmentObjectives"] = true;
            decisionTable.Rows[riskLevel]["RiskCapacity"] = true;
            decisionTable.Rows[riskLevel]["RiskAttitude"] = true;


            RiskTolerance suggestedRiskTolerance = decisionTable.SuggestedRiskTolerance;

            Assert.Equal(high, suggestedRiskTolerance.High);
            Assert.Equal(mediumHigh, suggestedRiskTolerance.MediumHigh);
            Assert.Equal(medium, suggestedRiskTolerance.Medium);
            Assert.Equal(lowMedium, suggestedRiskTolerance.LowMedium);
            Assert.Equal(low, suggestedRiskTolerance.Low);
        }

        // 0-based to match decision table index
        public static IEnumerable<object[]> SuggestedRiskToleranceData()
        {
            yield return new object[] {0,  0,  0,  0,  0, 100};
            yield return new object[] {1,  0,  0, 20, 80,   0};
            yield return new object[] {2,  0,  0, 50, 50,   0};
            yield return new object[] {3,  0, 20, 80,  0,   0};
            yield return new object[] {4, 40, 60,  0,  0,   0};
        }

        public static IEnumerable<object[]> SuggestedObjectivesData()
        {
            yield return new object[] {0,  0,   0,  0,  0, 100};
            yield return new object[] {1,  0,  30,  0, 70,   0};
            yield return new object[] {2,  0,  60,  0, 40,   0};
            yield return new object[] {3,  0, 100,  0,  0,   0};
            yield return new object[] {4, 20,  80,  0,  0,   0};
        }
    }
}
