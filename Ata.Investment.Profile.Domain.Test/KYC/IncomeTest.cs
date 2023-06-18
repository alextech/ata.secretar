using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.KYC
{
    public class IncomeTest
    {
        [Fact]
        public void AddIncomeObjectsTest()
        {
            // ReSharper disable InconsistentNaming
            Income income_1 = new Income(3, 1,"");
            Income income_2 = new Income(4, 1,"");

            Assert.Equal(7, income_1 + income_2);
        }
    }
}