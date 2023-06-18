using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Points.Tree;
using Ata.Investment.Profile.Domain.Points.Tree.Operation;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Profile
{
    public class CalculationVisitorTest
    {
        [Fact]
        public void SumExpressionTest()
        {
            IExpression expression = new Sum(
                new SingleValue(5),
                new SingleValue(6)
            );

            CalculationVisitor visitor = new CalculationVisitor();
            int total = visitor.Visit(expression);

            Assert.Equal(11, total);
        }

        [Fact]
        public void TotalCalculationTest()
        {
            const int goal = 3;
            const int decisionMaking = 10;
            IncomeAmount income = new IncomeAmount(new SingleValue(150000));

            IExpression expression =
                new Sum(
                    new Sum(
                        income,
                        new SingleValue(goal)),
                    new SingleValue(decisionMaking)
                );

            CalculationVisitor visitor = new CalculationVisitor();
            int total = visitor.Visit(expression);

            Assert.Equal(20, total);
        }
    }
}