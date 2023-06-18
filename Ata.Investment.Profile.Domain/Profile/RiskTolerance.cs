namespace Ata.Investment.Profile.Domain.Profile
{
    public class RiskTolerance
    {
        public RiskTolerance(int high, int mediumHigh, int medium, int lowMedium, int low)
        {
            if (high +
                mediumHigh +
                medium +
                lowMedium +
                low != 100
            )
                throw new BreakdownNotTotalingHundredException();

            High = high;
            MediumHigh = mediumHigh;
            Medium = medium;
            LowMedium = lowMedium;
            Low = low;
        }

        public int High { get; }
        public int MediumHigh { get; }
        public int Medium { get; }
        public int LowMedium { get; }
        public int Low { get; }
        public int Total => High + MediumHigh + Medium + LowMedium + Low;
    }
}