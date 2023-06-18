namespace Ata.Investment.Profile.Domain.Profile
{
    public class Objectives
    {
        public Objectives(int aggressiveGrowth, int growth, int balanced, int income, int cashReserve)
        {
            if (aggressiveGrowth +
                growth +
                balanced +
                income +
                cashReserve != 100
            )
                throw new BreakdownNotTotalingHundredException();

            AggressiveGrowth = aggressiveGrowth;
            Growth = growth;
            Balanced = balanced;
            Income = income;
            CashReserve = cashReserve;
        }

        public int AggressiveGrowth { get; }
        public int Growth { get; }
        public int Balanced { get; }
        public int Income { get; }
        public int CashReserve { get; }

        public int Total => AggressiveGrowth + Growth + Balanced + Income + CashReserve;

        public static bool operator ==(Objectives obj1, Objectives obj2)
        {

            return
                obj1.AggressiveGrowth == obj2.AggressiveGrowth &&
                obj1.Growth == obj2.Growth &&
                obj1.Balanced == obj2.Balanced &&
                obj1.Income == obj2.Income &&
                obj1.CashReserve == obj2.CashReserve;
        }

        public static bool operator !=(Objectives obj1, Objectives obj2)
        {
            return !(obj1 == obj2);
        }
    }
}