namespace Ata.Investment.Profile.Domain.Points.Tree.Operation
{
    public class AgeScore : IExpression
    {
        public IValue Value { get; }

        public AgeScore(IValue value)
        {
            Value = value;
        }
    }
}