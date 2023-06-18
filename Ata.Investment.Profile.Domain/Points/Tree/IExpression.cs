namespace Ata.Investment.Profile.Domain.Points.Tree
{
    public interface IExpression
    {
        public void Visit(Visitor visitor)
        {
            // visitor.(visitor);
        }
    }
}