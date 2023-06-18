namespace Ata.Investment.Profile.Domain.Points.Tree.Operation
{
    public class FinancialStability : IExpression
    {
        public IValue Value { get; }

        public FinancialStability(IValue value)
        {
            Value = value;
        }
    }
}