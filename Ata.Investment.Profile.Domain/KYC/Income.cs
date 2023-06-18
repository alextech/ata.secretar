namespace Ata.Investment.Profile.Domain.KYC
{
    public class Income
    {
        // MFDA: Q4
        public int Amount { get; set; }
        public string Notes { get; set; }

        // MFDA: Q5
        public int Stability { get; set; }

        public Income(int amount, int stability, string notes)
        {
            Amount = amount;
            Notes = notes;
            Stability = stability;
        }

        public static int operator +(Income income1, Income income2)
        {
            return income1.Amount + income2.Amount;
        }
    }
}