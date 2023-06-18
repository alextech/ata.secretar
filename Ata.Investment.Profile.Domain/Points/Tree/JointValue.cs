namespace Ata.Investment.Profile.Domain.Points.Tree
{
    public class JointValue : IValue
    {
        public int Primary { get; }
        public int Joint { get; }

        public JointValue(int primary, int joint)
        {
            Primary = primary;
            Joint = joint;
        }
    }
}