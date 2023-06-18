namespace Ata.Investment.Profile.Domain.Points.Tree.Operation
{
    public class FinancialSituation : IExpression
    {
        public IValue Value { get; }

        public FinancialSituation(IValue value)
        {
            Value = value;
        }
    }
}