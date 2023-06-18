using System;
using System.Globalization;
using Moq;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.KYC
{
    public class PClientTest
    {
        [Fact]
        public void CalculateAgeTest()
        {
            Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 05, 20));
            TimeProvider.Current = timeMock.Object;

            PClient client = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.ParseExact("14/04/1989", "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            Assert.Equal(30, client.Age);
        }

        [Fact]
        public void JointPClientShareNetWorth()
        {
            // === SETUP ===
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));
            PClient joint = new PClient(new Client("joint", "joint@pclient.test", DateTimeOffset.MinValue));
            primary.NetWorth = new NetWorth(3, 2, 1);

            // === ACT ===
            primary.JointWith = joint;

            // === VERIFY ===
            Assert.Same(primary.NetWorth, joint.NetWorth);
            Assert.Equal(2, primary.NetWorth.FixedAssets);
            Assert.Equal(2, joint.NetWorth.FixedAssets);

            // === ACT ===
            joint.NetWorth.FixedAssets = 4;

            // === VERIFY ===
            Assert.Equal(4, primary.NetWorth.FixedAssets);
            Assert.Equal(4, joint.NetWorth.FixedAssets);
        }

        [Fact]
        public void JointPClientKnowsWhoIsJointWith()
        {
            // === SETUP ===
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));
            PClient joint = new PClient(new Client("joint", "joint@pclient.test", DateTimeOffset.MinValue));
            primary.NetWorth = new NetWorth(3, 2, 1);

            // === ACT ===
            primary.JointWith = joint;

            // === VERIFY ===
            Assert.Same(primary, joint.JointWith);
        }

        [Fact]
        public void JointWithSelfHasNoEffect()
        {
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));
            primary.JointWith = primary;

            Assert.Null(primary.JointWith);
        }

        [Fact]
        public void SettingJointToNullDecouplesNetWorth()
        {
            // === SETUP ===
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));
            PClient joint = new PClient(new Client("joint", "joint@pclient.test", DateTimeOffset.MinValue));
            primary.NetWorth = new NetWorth(3, 2, 1);
            primary.JointWith = joint;

            // === ACT ===
            primary.JointWith = null;
            joint.NetWorth.FixedAssets = 4;

            // === VERIFY ===
            Assert.Equal(new NetWorth(3, 2, 1), primary.NetWorth);
        }

        [Fact]
        public void SettingJointToNullAlsoSetsNullOnJoint()
        {
            // === SETUP ===
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));
            PClient joint = new PClient(new Client("joint", "joint@pclient.test", DateTimeOffset.MinValue));
            primary.JointWith = joint;

            // === ACT ===
            primary.JointWith = null;

            // === VERIFY ===
            Assert.Null(joint.JointWith);
        }

        [Fact]
        public void HandleSettingJointToNull_AlreadyNull()
        {
            // === SETUP ===
            PClient primary = new PClient(new Client("primary", "primary@pclient.test", DateTimeOffset.MinValue));

            // === ACT+VERIFY ===
            primary.JointWith = null;
        }
    }
}