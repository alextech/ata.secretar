using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Profile
{
    public class NetWorthTest
    {
        [Fact]
        public void CloneTest()
        {
            NetWorth netWorth = new NetWorth(10, 5, 7);

            Assert.Equal(netWorth, netWorth.Clone());
        }

        [Fact]
        public void TotalTest()
        {
            NetWorth netWorth = new NetWorth(10, 5, 7);

            Assert.Equal(8, netWorth.Total);
        }
    }
}