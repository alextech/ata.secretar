namespace Ata.Investment.Profile.Domain.Points
{
    public class Allocation
    {
        public Allocation(string name, int riskLevel) =>
            (Name, RiskLevel) = (name, riskLevel);

        public string Name { get; private set; }
        public int RiskLevel { get; private set; }
    }
}