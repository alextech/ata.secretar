using System;
using System.Collections;
using System.Linq;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.KYC
{
    public class KycDocumentTest
    {
        [Fact]
        public void DeleteProfileTest()
        {
            // ==== setup =====
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.PrimaryClient);
            document.PrimaryClient.AddProfile(profile);

            profile = new Domain.Profile.Profile(document.JointClient);
            document.JointClient.AddProfile(profile);

            profile = new Domain.Profile.Profile(document.PrimaryClient, document.JointClient);
            document.AddJointProfile(profile);

            // ==== act =====
            document.RemoveProfile(profile);

            // === verify ====
            Assert.Single(document.PrimaryClient.Profiles);
            Assert.Single(document.JointClient.Profiles);
            Assert.Empty(document.JointProfiles);
        }

        private static readonly Domain.Household.Household JointHousehold = new Domain.Household.Household(
            new Client("c_1", "c1@kycext.test", DateTimeOffset.MinValue),
            new Client("c_2", "c2@kycext.test", DateTimeOffset.MinValue)
        );

        [Fact]
        public void GetAllProfilesTest()
        {
            // ==== setup =====
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.PrimaryClient);
            document.PrimaryClient.AddProfile(profile);

            profile = new Domain.Profile.Profile(document.JointClient);
            document.JointClient.AddProfile(profile);

            profile = new Domain.Profile.Profile(document.PrimaryClient, document.JointClient);
            document.AddJointProfile(profile);

            // === verify ====
            Assert.Equal(3, document.AllProfiles.Count());
        }

        [Fact]
        public void ProfileAdditionTriggersEvent()
        {
            // ==== setup =====
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            bool wasCalled;
            document.ProfileListChanged += (s, e) => wasCalled = true;

            // ==== act + verify ====
            wasCalled = false;
            Domain.Profile.Profile profile = new Domain.Profile.Profile(document.PrimaryClient);
            document.PrimaryClient.AddProfile(profile);
            Assert.True(wasCalled);
            wasCalled = false;
            document.PrimaryClient.RemoveProfile(profile);
            Assert.True(wasCalled);

            wasCalled = false;

            profile = new Domain.Profile.Profile(document.JointClient);
            document.JointClient.AddProfile(profile);
            Assert.True(wasCalled);
            wasCalled = false;
            document.JointClient.RemoveProfile(profile);
            Assert.True(wasCalled);

            wasCalled = false;

            profile = new Domain.Profile.Profile(document.PrimaryClient, document.JointClient);
            document.AddJointProfile(profile);
            Assert.True(wasCalled);
            wasCalled = false;
            document.RemoveProfile(profile);
            Assert.True(wasCalled);
        }

        [Fact]
        public void RemovingClientFromDocumentRemovesEventListener()
        {
            // ==== setup =====
            KycDocument document = new KycDocument(Guid.Empty, null, JointHousehold, 1803);
            bool wasCalled = false;
            document.ProfileListChanged += (s, e) => wasCalled = true;
            PClient danglingClient = document.PrimaryClient;
            document.SwitchToSingleWith(document.JointClient);

            danglingClient.AddProfile(new Domain.Profile.Profile(danglingClient));
            Assert.False(wasCalled);

        }
    }
}