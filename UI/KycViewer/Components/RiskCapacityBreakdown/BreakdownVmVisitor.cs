using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Points.Tree;
using Ata.Investment.Profile.Domain.Points.Tree.Operation;
using Ata.Investment.Profile.Domain.Profile;

namespace KycViewer.Components.RiskCapacityBreakdown
{
    public class BreakdownVmVisitor : Visitor
    {
        public List<string[]> Rows { get; } = new List<string[]>();
        // mostly for networth label, which is not identifiable through object alone
        // but could also take opportunity to assert all other values match join state
        public bool ForJoint { private get; init; }

        private static readonly string PrimaryPlaceholder = new Guid(1, 0, 0, new byte[8]).ToString();
        private static readonly string JointPlaceholder = new Guid(2, 0, 0, new byte[8]).ToString();

        public override int Visit<T>(T expression)
        {
            return expression switch
            {
                AgeScore ageScore => Visit(ageScore),

                IncomeAmount incomeAmount => Visit(incomeAmount),
                FinancialStability financialStability => Visit(financialStability),
                FinancialSituation financialSituation => Visit(financialSituation),

                NetWorth netWorth => Visit(netWorth),

                Sum sum => Visit(sum),
                SingleValue singleValue => Visit(singleValue),
                _ => throw new ArgumentOutOfRangeException(expression?.GetType().Name)
            };
        }

        private int Visit(AgeScore ageScore)
        {
            int points;
            if (ageScore.Value.GetType() == typeof(JointValue))
            {
                Debug.Assert(ForJoint, "ForJoint");
                JointValue value = (JointValue) ageScore.Value;

                int primaryScore = ProfilePoints.AgeScore(value.Primary);
                int jointScore = ProfilePoints.AgeScore(value.Joint);

                points = Math.Min(primaryScore, jointScore);

                Rows.Add(new [] { "groupSubItem", "Age", "", "", "", "" });
                Rows.Add(new [] { "jointSubItem", $"Primary (age {value.Primary})", primaryScore.ToString(), "", "", "age" });
                Rows.Add(new [] { "jointSubItem", $"Joint (age {value.Joint})", jointScore.ToString(), "", "", "age" });
                Rows.Add(new [] { "subtotalRow", "", "MIN()", points.ToString(), "", "" });
            }
            else
            {
                Debug.Assert(!ForJoint, "!ForJoint");

                SingleValue value = (SingleValue) ageScore.Value;

                points = ProfilePoints.AgeScore(value.Value);

                Rows.Add(new []{ "groupSubItem", $"Age ({value.Value})", points.ToString(), "", "age" });
            }

            return points;
        }

        private int Visit(IncomeAmount incomeAmount)
        {
            int points;
            if (incomeAmount.Value.GetType() == typeof(JointValue))
            {
                Debug.Assert(ForJoint, "ForJoint");

                JointValue value = (JointValue) incomeAmount.Value;
                points = ProfilePoints.IncomeAmountScore(value.Primary + value.Joint);

                Rows.Add(new []{ "groupSubItem", "Combined annual income", "", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/incomeAmount" });
            }
            else
            {
                Debug.Assert(!ForJoint, "!ForJoint");

                points = ProfilePoints.IncomeAmountScore(((SingleValue) incomeAmount.Value).Value);

                Rows.Add(new []{ "groupSubItem", "Annual income", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/incomeAmount" });
            }

            return points;
        }

        private int Visit(FinancialStability financialStability)
        {
            int points;
            if (financialStability.Value.GetType() == typeof(JointValue))
            {
                Debug.Assert(ForJoint, "ForJoint");

                JointValue value = ((JointValue) financialStability.Value);
                points = Math.Min(value.Primary, value.Joint);

                Rows.Add(new [] { "groupSubItem", "Financial stability", "", "", "", "" });
                Rows.Add(new [] { "jointSubItem", "Primary", value.Primary.ToString(), "", "", $"/client/{PrimaryPlaceholder}/info/incomeStability" });
                Rows.Add(new [] { "jointSubItem", "Joint", value.Joint.ToString(), "", "", $"/client/{JointPlaceholder}/info/incomeStability" });
                Rows.Add(new [] { "subtotalRow", "", "MIN()", points.ToString(), "", "" });
            }
            else
            {
                Debug.Assert(!ForJoint, "!ForJoint");

                SingleValue value = (SingleValue) financialStability.Value;
                points = value.Value;

                Rows.Add(new []{ "groupSubItem", "Financial stability", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/incomeStability" });
            }

            return points;
        }

        private int Visit(FinancialSituation financialSituation)
        {
            int points;
            if (financialSituation.Value.GetType() == typeof(JointValue))
            {
                Debug.Assert(ForJoint, "ForJoint");

                JointValue value = (JointValue) financialSituation.Value;

                points = Math.Min(value.Primary, value.Joint);

                Rows.Add(new [] { "groupSubItem", "Financial situation", "", "", "", "" });
                Rows.Add(new [] { "jointSubItem", "Primary", value.Primary.ToString(), "", "", $"/client/{PrimaryPlaceholder}/info/financialSituation" });
                Rows.Add(new [] { "jointSubItem", "Joint", value.Joint.ToString(), "", "", $"/client/{JointPlaceholder}/info/financialSituation" });
                Rows.Add(new [] { "subtotalRow", "", "MIN()", points.ToString(), "", "" });
            }
            else
            {
                Debug.Assert(!ForJoint, "!ForJoint");

                SingleValue value = (SingleValue) financialSituation.Value;

                points = value.Value;

                Rows.Add(new []{ "groupSubItem", "Financial situation", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/financialSituation" });
            }

            return points;
        }

        private int Visit(NetWorth netWorth)
        {
            int points = ProfilePoints.NetworthScore(netWorth);
            Rows.Add(ForJoint
                ? new[] {"groupSubItem", "Shared networth", "", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/networth"}
                : new[] {"groupSubItem", "Networth", points.ToString(), "", $"/client/{PrimaryPlaceholder}/info/networth"});

            return points;
        }

        private int Visit(Sum sum) =>
            Visit(sum.Left) + Visit(sum.Right);

        private static int Visit(SingleValue singleValue)
            => singleValue.Value;
    }
}