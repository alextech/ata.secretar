using System;
using Ata.Investment.Profile.Domain.Points.Tree;
using Ata.Investment.Profile.Domain.Points.Tree.Operation;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.Profile.Domain.Points
{
    public class CalculationVisitor : Visitor
    {

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

        private static int Visit(AgeScore ageScore)
        {
            int age;

            if (ageScore.Value.GetType() == typeof(JointValue))
            {
                JointValue value = (JointValue) ageScore.Value;
                age = Math.Max(value.Primary, value.Joint);
            }
            else
            {
                age = ((SingleValue) ageScore.Value).Value;
            }

            return ProfilePoints.AgeScore(age);
        }

        private int Visit(IncomeAmount incomeAmount)
        {
            int amount;
            if (incomeAmount.Value.GetType() == typeof(JointValue))
            {
                JointValue value = (JointValue) incomeAmount.Value;
                amount = value.Primary + value.Joint;
            }
            else
            {
                amount = ((SingleValue) incomeAmount.Value).Value;
            }

            return ProfilePoints.IncomeAmountScore(amount);
        }

        private static int Visit(FinancialStability financialStability)
        {
            if (financialStability.Value.GetType() == typeof(JointValue))
            {
                JointValue value = (JointValue) financialStability.Value;
                return Math.Min(value.Primary, value.Joint);
            }

            return ((SingleValue) financialStability.Value).Value;
        }

        private static int Visit(FinancialSituation financialSituation)
        {
            if (financialSituation.Value.GetType() == typeof(JointValue))
            {
                JointValue value = (JointValue) financialSituation.Value;
                return Math.Min(value.Primary, value.Joint);
            }

            return ((SingleValue) financialSituation.Value).Value;
        }

        private static int Visit(NetWorth netWorth)
            => ProfilePoints.NetworthScore(netWorth);

        private int Visit(Sum sum) =>
            Visit(sum.Left) + Visit(sum.Right);

        private int Visit(SingleValue value) =>
            value.Value;


    }
}