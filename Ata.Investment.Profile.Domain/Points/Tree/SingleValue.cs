namespace Ata.Investment.Profile.Domain.Points.Tree
{
    public class SingleValue : IValue
    {
        public int Value { get; }

        public SingleValue(int value)
        {
            Value = value;
        }
    }
}