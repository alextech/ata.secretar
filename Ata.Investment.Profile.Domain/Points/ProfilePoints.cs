using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points.Tree;
using Ata.Investment.Profile.Domain.Points.Tree.Operation;
using Ata.Investment.Profile.Domain.Profile;
using Range = SharedKernel.Range;

// ReSharper disable JoinDeclarationAndInitializer

namespace Ata.Investment.Profile.Domain.Points
{
    public static class ProfilePoints
    {

        public static DecisionTable CreateDecisionTable(this Profile.Profile profile)
        {
            RiskTable riskTable = new RiskTable();
            EnumerableRowCollection<DataRow> riskTableEnum = riskTable.AsEnumerable();

            // make breakdown table somewhere here
            // make decision table based on it

            DecisionTable decisionTable = new DecisionTable(profile);
            DecisionBreakdown decisionBreakdown = decisionTable.DecisionBreakdown;

            EnumerableRowCollection<DataRow> rows = decisionTable.AsEnumerable();

            int riskLevel;
            // TIME HORIZON
            riskLevel = (
                from row in riskTableEnum
                where row.Field<Range>("TimeHorizon").Min <= profile.TimeHorizon.WithdrawTime &&
                      row.Field<Range>("TimeHorizon").Max >= profile.TimeHorizon.WithdrawTime
                select row.Field<int>("RiskLevel")
            ).Single();
            rows.Single(r => r.Field<int>("RiskLevel") == riskLevel)["TimeHorizon"] = true;
            decisionBreakdown.TimeHorizon = profile.TimeHorizon.WithdrawTime;

            // KNOWLEDGE
            riskLevel = (
                from row in riskTableEnum
                where row.Field<Range>("InvestmentKnowledge").Min <= profile.Knowledge.Answer.Level &&
                      row.Field<Range>("InvestmentKnowledge").Max >= profile.Knowledge.Answer.Level
                select row.Field<int>("RiskLevel")
            ).Single();
            rows.Single(r => r.Field<int>("RiskLevel") == riskLevel)["InvestmentKnowledge"] = true;
            decisionBreakdown.InvestmentKnowledge = profile.Knowledge.Answer.Level;

            // OBJECTIVES
            riskLevel = (
                from row in riskTableEnum
                where row.Field<int>("InvestmentObjectives") == profile.Goal
                select row.Field<int>("RiskLevel")
            ).Single();
            rows.Single(r => r.Field<int>("RiskLevel") == riskLevel)["InvestmentObjectives"] = true;
            decisionBreakdown.InvestmentObjectives = profile.Goal;

            // RISK CAPACITY
            (int Points, IExpression Breakdown) capacity = RiskCapacityPoints(profile);
            riskLevel = (
                from row in riskTableEnum
                where row.Field<Range>("RiskCapacity").Min <= capacity.Points &&
                      row.Field<Range>("RiskCapacity").Max >= capacity.Points
                select row.Field<int>("RiskLevel")
            ).Single();
            rows.Single(r => r.Field<int>("RiskLevel") == riskLevel)["RiskCapacity"] = true;
            decisionBreakdown.RiskCapacity = capacity.Breakdown;

            // RISK ATTITUDE
            (int Points, Dictionary<string, int> Breakdown) attitude = RiskAttitudePoints(profile);
            riskLevel = (
                from row in riskTableEnum
                where row.Field<Range>("RiskAttitude").Min <= attitude.Points &&
                      row.Field<Range>("RiskAttitude").Max >= attitude.Points
                select row.Field<int>("RiskLevel")
            ).Single();
            rows.Single(r => r.Field<int>("RiskLevel") == riskLevel)["RiskAttitude"] = true;
            decisionBreakdown.RiskAttitude = attitude.Breakdown;

            return decisionTable;
        }

        #region PropertyScores

        public static int AgeScore(int age)
        {
            return age switch
            {
                _ when age <= 34 => 20,
                _ when age <= 54 => 8,
                _ when age <= 64 => 3,
                _ => 1
            };
        }

        public static int IncomeAmountScore(int incomeAmount)
        {
            return true switch
            {
                _ when incomeAmount < 25000 => 0,
                _ when incomeAmount <= 49999 => 2,
                _ when incomeAmount <= 74999 => 4,
                _ when incomeAmount <= 99999 => 5,
                _ when incomeAmount <= 199999 => 7,
                _ => 10
            };
        }

        public static int NetworthScore(NetWorth netWorth)
        {
            int total = netWorth.Total;

            return total switch
            {
                _ when total < 40000  => 0,
                _ when total <= 99999  => 2,
                _ when total <= 199999 => 4,
                _ when total <= 500000 => 6,
                _ when total <= 999999 => 8,
                _ => 10
            };
        }
        private static int SelectionScore(int selection)
        {
            return selection >= 0 ? selection : 0;
        }
        #endregion

        private static (int, IExpression) RiskCapacityPoints(Profile.Profile profile)
        {
            PClient primary = profile.PrimaryClient;
            PClient? joint = profile.JointClient;

            IExpression sum;
            sum = new Sum(
                new FinancialStability(
                    profile.IsJoint
                        ? (IValue) new JointValue(primary.Income.Stability, joint.Income.Stability)
                        : new SingleValue(primary.Income.Stability)
                ),
                profile.NetWorth
            );

            sum = new Sum(
                sum,
                new FinancialSituation(
                    profile.IsJoint
                        ? (IValue) new JointValue(primary.FinancialSituation, joint.FinancialSituation)
                        : new SingleValue(primary.FinancialSituation)
                )
            );

            sum = new Sum(
                sum,
                new SingleValue(profile.PercentageOfSavings)
            );

            sum = new Sum(
                sum,
                new IncomeAmount(
                    profile.IsJoint
                        ? (IValue) new JointValue(primary.Income.Amount, joint.Income.Amount)
                        : new SingleValue(primary.Income.Amount)
                )
            );

            sum = new Sum(
                sum,
                new AgeScore(
                    profile.IsJoint ? (IValue) new JointValue(primary.Age, joint.Age) : new SingleValue(primary.Age)
                )
            );

            CalculationVisitor visitor = new CalculationVisitor();
            int points = visitor.Visit(sum);

            return (
                points,
                sum
            );
        }

        private static (int, Dictionary<string, int>) RiskAttitudePoints(Profile.Profile profile)
        {
            return (
                SelectionScore(profile.DecisionMaking) +
                SelectionScore(profile.Decline) +
                SelectionScore(profile.ActionOnLosses) +
                SelectionScore(profile.LossesOrGains) +
                SelectionScore(profile.LossVsGainProfile) +
                SelectionScore(profile.HypotheticalProfile),

                new Dictionary<string, int>
                {
                    { "decisionMaking", profile.DecisionMaking },
                    { "decline", profile.Decline },
                    { "actionOnLosses", profile.ActionOnLosses},
                    { "lossesOrGains", profile.LossesOrGains },
                    { "lossvsgainprofile", profile.LossVsGainProfile },
                    { "hypotheticalProfile", profile.HypotheticalProfile}
                }
            );
        }

        public static int RoundTimeHorizon(int years)
        {
            return years switch
            {
                <  1 => 0,
                <= 3 => 1,
                <= 7 => 4,
                <= 15 => 8,
                _ => 16
            };
        }

        public static Range MatchRiskRange(int minYear)
        {
            return minYear switch {
                0 => new Range(0, 0),
                1 => new Range(1, 3),
                4 => new Range(4, 6),
                7 => new Range(7, 9),
                _ => new Range(10, 20)
            };
        }
    }
}
