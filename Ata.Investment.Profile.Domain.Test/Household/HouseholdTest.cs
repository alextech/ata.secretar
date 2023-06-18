using System;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Household
{
    public class HouseholdTest
    {

        [Fact]
        public void IsSingleTest()
        {
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);

            Assert.False(h.IsJoint);
        }

        [Fact]
        public void IsJointTest()
        {
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);

            Domain.Household.Household h = new Domain.Household.Household(c1, c2);

            Assert.True(h.IsJoint);
        }

        [Fact]
        public void CanAddMemberToHousehold()
        {
            // setup
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);

            // act
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);
            h.AddMember(c2);

            // verify
            Assert.Equal(c1.Guid, h.PrimaryClient.Guid);
            Assert.Equal(c2.Guid, h.JointClient.Guid);
        }

        [Fact]
        public void CanRemoveJointFromHousehold()
        {
            // setup
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);
            h.AddMember(c2);
            Assert.True(h.IsJoint);

            // act
            h.RemoveMember(c1);

            // verify
            Assert.False(h.IsJoint);
        }

        [Fact]
        public void RemovingPrimaryMakesJointPrimary()
        {
            // setup
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);
            h.AddMember(c2);
            Assert.True(h.IsJoint);

            // act
            h.RemoveMember(c1);

            // verify
            Assert.Equal(c2.Guid, h.PrimaryClient.Guid);
            Assert.False(h.IsJoint);
        }

        [Fact]
        public void AttemptToRemoveClientNotInHouseholdThrowsException()
        {
            // setup
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);

            Assert.Throws<InvalidClientRemovalException>(() => h.RemoveMember(c2));
        }

        [Fact]
        public void RemoveClientById()
        {
            // setup
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);
            h.AddMember(c2);
            Assert.True(h.IsJoint);

            // act
            h.RemoveMember(c1.Guid);

            // verify
            Assert.False(h.IsJoint);
        }

        [Fact]
        public void AddingMemberToJointHouseholdThrowsException()
        {
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);

            Domain.Household.Household h = new Domain.Household.Household(c1, c2);

            Assert.Throws<InvalidClientAdditionException>(() => h.AddMember(c2));
        }

        [Fact]
        public void CanBeginMeeting()
        {
            Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            Domain.Household.Household h = new Domain.Household.Household(c1);

            Meeting meeting = h.BeginMeeting(new Advisor(Guid.Empty, "", "", ""), 1803);

            Assert.IsType<Meeting>(meeting);
            Assert.IsType<KycDocument>(meeting.KycDocument);
            Assert.Equal(c1.Guid, meeting.KycDocument.PrimaryClient.Guid);
        }
    }
}