using System;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test
{
    public class KycDocumentExtensionsTest
    {
        // [Theory]
        // [MemberData(nameof(Households))]
        [Fact]
        public void GetClientByIdSingleHousehold_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, SingleHousehold, 1803);
            Guid c1Id = SingleHousehold.PrimaryClient.Guid;

            Assert.Equal(c1Id, document.GetClientById(c1Id).Guid);
        }

        [Fact]
        public void GetClientFromJointHousehold_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            Guid c1Id = JointHousehold.PrimaryClient.Guid;
            Guid c2Id = JointHousehold.JointClient.Guid;

            Assert.Equal(c1Id, document.GetClientById(c1Id).Guid);
            Assert.Equal(c2Id, document.GetClientById(c2Id).Guid);
        }

        [Fact]
        public void GettingNonExistentClientThrowsException_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, SingleHousehold, 1803);
            Assert.Throws<NonExistentClientRequestedException>(
                () => document.GetClientById(Guid.Empty)
            );
        }

        [Fact]
        public void GettingSingleProfileFromSingle_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, SingleHousehold, 1803);
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.PrimaryClient);
            document.PrimaryClient.AddProfile(profile);

            Assert.Equal(profile.Guid, document.GetProfileById(profile.Guid).Guid);
        }

        [Fact]
        public void GettingSingleProfileFromJoint_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.JointClient);
            document.JointClient.AddProfile(profile);

            Assert.Equal(profile.Guid, document.GetProfileById(profile.Guid).Guid);
        }

        [Fact]
        public void GettingJointProfile_Test()
        {
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.JointClient);
            document.AddJointProfile(profile);

            Assert.Equal(profile.Guid, document.GetProfileById(profile.Guid).Guid);
        }

        private static readonly Domain.Household.Household SingleHousehold = new Domain.Household.Household(
            new Client("c_1", "c1@kycext.test", DateTimeOffset.MinValue)
        );
        private static readonly Domain.Household.Household JointHousehold = new Domain.Household.Household(
            new Client("c_1", "c1@kycext.test", DateTimeOffset.MinValue),
            new Client("c_2", "c2@kycext.test", DateTimeOffset.MinValue)
        );

    }
}