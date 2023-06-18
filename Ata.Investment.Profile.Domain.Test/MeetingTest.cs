using System;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test
{
    public class MeetingTest
    {
        [Fact]
        public void BeginningMeetingCreatesKycDocument()
        {
            // === setup ===
            Domain.Household.Household household = new Domain.Household.Household(
                new Client("c", "0", DateTimeOffset.MinValue)
            );
            Meeting meeting = new Meeting(household, new Advisor(Guid.Empty, "", "", ""), 1803);

            // === act ===
            meeting.Begin();

            // === verify ===
            Assert.IsType<KycDocument>(meeting.KycDocument);
            Assert.Equal(1803, meeting.KycDocument.AllocationVersion);
        }

        [Fact]
        public void SwitchingAdvisor()
        {
            // === setup ===
            Domain.Household.Household household = new Domain.Household.Household(
                new Client("c", "0", DateTimeOffset.MinValue)
            );
            Meeting meeting = new Meeting(household, new Advisor(Guid.Empty, "", "", ""), 1803);

            // === act ===
            meeting.Begin();
            Advisor newAdvisor = new Advisor(Guid.NewGuid(), "switched", "", "switched@test.local");
            meeting.SwitchAdvisor(newAdvisor);

            // === verify ===
            Assert.Equal(newAdvisor.Guid, meeting.AdvisorGuid);
            Assert.Equal(newAdvisor.Guid, meeting.KycDocument.Advisor.Guid);
        }

        [Fact]
        public void SwitchingAdvisorInMeetingAlsoSwitchesDocument()
        {
            // === setup ===
            Domain.Household.Household household = new Domain.Household.Household(
                new Client("c", "0", DateTimeOffset.MinValue)
            );
            Meeting meeting = new Meeting(household, new Advisor(Guid.Empty, "", "", ""), 1803);

            // === act ===
            meeting.Begin();
            Advisor newAdvisor = new Advisor(Guid.NewGuid(), "switched", "", "switched@test.local");
            meeting.SwitchAdvisor(newAdvisor);

            // === verify ===
            Assert.Equal(newAdvisor.Guid, meeting.KycDocument.Advisor.Guid);
        }
    }
}