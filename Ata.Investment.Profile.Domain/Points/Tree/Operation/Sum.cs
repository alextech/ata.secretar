namespace Ata.Investment.Profile.Domain.Points.Tree.Operation
{
    public class Sum : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Sum(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
    }
}