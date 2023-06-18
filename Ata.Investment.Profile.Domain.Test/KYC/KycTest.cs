using System;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.KYC
{
    public class KycTest
    {
        [Fact]
        public void ByDefaultCreatesProfileMatchingHousehold()
        {
            // === setup ===
            Domain.Household.Household jointHousehold = new Domain.Household.Household(
                new Client("", "", DateTimeOffset.Now),
                new Client("", "", DateTimeOffset.Now)
            );
            KycDocument kycForJoint = new KycDocument(Guid.Empty, null, jointHousehold, 1803);

            Domain.Household.Household singleHousehold = new Domain.Household.Household(
                new Client("", "", DateTimeOffset.Now)
            );
            KycDocument kycForSingle = new KycDocument(Guid.Empty, null, singleHousehold, 1803);

            // === act+verify ===
            Assert.True(kycForJoint.IsJoint);
            Assert.False(kycForSingle.IsJoint);
        }

        [Fact]
        public void CanSwitchBetweenJointAndSingleIfPossible()
        {
            Domain.Household.Household jointHousehold = new Domain.Household.Household(
                new Client("p", "", DateTimeOffset.Now),
                new Client("j", "", DateTimeOffset.Now)
            );
            KycDocument kyc = new KycDocument(Guid.Empty, null, jointHousehold, 1803);

            kyc.SwitchToSingleWith(new PClient(jointHousehold.JointClient));
            Assert.False(kyc.IsJoint);
            Assert.Equal(jointHousehold.JointClient.Guid, kyc.PrimaryClient.Guid);

            kyc.SwitchToJointWith(new PClient(jointHousehold.PrimaryClient));
            Assert.True(kyc.IsJoint);

            // they would now be exchanged because joint was promoted to primary, and primary was later re-added as joint
            Assert.Equal("p", kyc.JointClient.Name);
        }

        // needed for getting networth
        [Fact]
        public void SwitchingToJointSetsPrimaryClientWithJointProperty()
        {
            Domain.Household.Household household = new Domain.Household.Household(
                new Client("p", "", DateTimeOffset.Now)
            );
            KycDocument kyc = new KycDocument(Guid.Empty, null, household, 1803);
            PClient newJoint = new PClient(new Client("j", "", DateTimeOffset.Now));

            kyc.SwitchToJointWith(newJoint);
            Assert.Equal(newJoint.Guid, kyc.PrimaryClient.JointWith.Guid);
        }

        [Fact]
        public void SwitchingToSingleSetsPrimaryClientJointWithToNull()
        {
            Domain.Household.Household household = new Domain.Household.Household(
                new Client("p", "", DateTimeOffset.Now),
                new Client("j", "", DateTimeOffset.Now)
            );
            KycDocument kyc = new KycDocument(Guid.Empty, null, household, 1803);

            kyc.SwitchToSingleWith(new PClient(household.PrimaryClient));
            Assert.Null(kyc.PrimaryClient.JointWith);
        }

        [Fact]
        public void IfHouseholdNotJointCannotSwitchToJoint()
        {

        }
    }
}