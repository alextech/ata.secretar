namespace Ata.Investment.Profile.Domain.Points.Tree.Operation
{
    public class IncomeAmount : IExpression
    {
        public IValue Value { get; }

        public IncomeAmount(IValue value)
        {
            Value = value;
        }
    }
}