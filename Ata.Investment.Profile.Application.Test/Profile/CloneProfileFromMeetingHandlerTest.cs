using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ata.Investment.Profile.Application.Profile;
using Ata.Investment.Profile.Cmd.Advisors;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Application.Test.Profile
{
    public class CloneProfileFromMeetingHandlerTest
    {

        private Mock<IMediator> _mockMediator;
        private Guid _advisorGuid;
        public CloneProfileFromMeetingHandlerTest()
        {
            _advisorGuid = Guid.NewGuid();

            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(m => m.Send(It.IsAny<AdvisorByIdQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Advisor>, CancellationToken>((notification, cToken) => {})
                .ReturnsAsync(new Advisor(_advisorGuid, "", "", ""));
        }

        [Fact]
        public async Task MeetingFromExistingMeetingTest()
        {
            DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
                .UseInMemoryDatabase(databaseName: "meetings_test_db")
                .Options;

            Meeting prevMeeting;

            Guid c1Id;

            // ========= SETUP ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);

                Client c1 = new Client("c_1", "c1@clone_meeting.test", DateTimeOffset.MinValue);
                Client c2 = new Client("c_2", "c2@clone_meeting.test", DateTimeOffset.MinValue);
                Household h = new Household(c1, c2);
                prevMeeting = h.BeginMeeting(new Advisor(Guid.NewGuid(), "", "TST", ""), 1803);

                c1Id = c1.Guid;

                KycDocument doc = prevMeeting.KycDocument;
                doc.PrimaryClient.Income = new Income(500, 1, "tst_income");
                doc.PrimaryClient.Knowledge = new Knowledge(2, "tst_knowledge");
                // doc.PrimaryClient.NetWorth = new NetWorth(3, 2, 1, "tst_networth");

                doc.PrimaryClient.NetWorth.LiquidAssets = 3;
                doc.PrimaryClient.NetWorth.FixedAssets = 2;
                doc.PrimaryClient.NetWorth.Liabilities = 1;
                doc.PrimaryClient.NetWorth.Notes = "tst_networth";

                Debug.Assert(doc.JointClient != null, "doc.JointClient != null");
                doc.JointClient.Income = new Income(501, 1, "tst_income");

                Domain.Profile.Profile profile = new Domain.Profile.Profile(doc.PrimaryClient)
                {
                    Name = "Tst_Profile",
                    Accounts = new Accounts {LIRA = true},
                    Decline = 10,
                    InitialInvestment = 13,
                    MonthlyCommitment = 14,
                    TimeHorizon = new TimeHorizon(2020){Range = new SharedKernel.Range(2030, 2030)}
                };
                doc.PrimaryClient.AddProfile(profile);
                doc.PrimaryClient.Income.Notes = "primary income notes";
                doc.PrimaryClient.NetWorth.Notes = "networth notes";
                doc.PrimaryClient.Knowledge.Notes = "primary knowledge notes";

                await repo.AddAsync(prevMeeting);
                await context.SaveChangesAsync();
            }

            Guid newMeetingId;
            // ========= ACT / ASSERT ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);
                Meeting meeting = await repo.FindByIdAsync(prevMeeting.Guid);
                Assert.Equal(prevMeeting.Guid, meeting.Guid);

                CloneProfileFromMeetingCommand cmd = new CloneProfileFromMeetingCommand(prevMeeting.Guid);
                CloneProfileFromMeetingCommandHandler commandHandler = new CloneProfileFromMeetingCommandHandler(repo, _mockMediator.Object);

                newMeetingId = (await commandHandler.Handle(cmd, CancellationToken.None)).Data;
            }

            // ========= VERIFY =============
            await using (ProfileContext context = new ProfileContext(options))
            {
                Assert.NotEqual(prevMeeting.Guid, newMeetingId);
                Assert.NotEqual(Guid.Empty, newMeetingId);

                MeetingRepository repo = new MeetingRepository(context);
                Meeting newMeeting = await repo.FindByIdAsync(newMeetingId);

                Assert.NotNull(newMeeting);
                Assert.Equal(newMeetingId, newMeeting.Guid);

                KycDocument newDoc = newMeeting.KycDocument;

                Assert.Equal(500, newDoc.PrimaryClient.Income.Amount);
                Assert.Equal(2, newDoc.PrimaryClient.Knowledge.Level);

                Assert.Equal(3, newDoc.PrimaryClient.NetWorth.LiquidAssets);
                Assert.Equal(2, newDoc.PrimaryClient.NetWorth.FixedAssets);
                Assert.Equal(1, newDoc.PrimaryClient.NetWorth.Liabilities);

                Assert.Equal("primary income notes", newDoc.PrimaryClient.Income.Notes);
                Assert.Equal("networth notes", newDoc.PrimaryClient.NetWorth.Notes);
                Assert.Equal("primary knowledge notes", newDoc.PrimaryClient.Knowledge.Notes);

                Assert.True(newDoc.IsJoint);

                Debug.Assert(newDoc.JointClient != null, "newDoc.JointClient != null");
                Assert.Equal(501, newDoc.JointClient.Income.Amount);

                Assert.Equal(3, newDoc.JointClient.NetWorth.LiquidAssets);
                Assert.Equal(2, newDoc.JointClient.NetWorth.FixedAssets);
                Assert.Equal(1, newDoc.JointClient.NetWorth.Liabilities);

                // others are done in same way

                Domain.Profile.Profile profile = newDoc.PrimaryClient.Profiles.First();
                Assert.Equal(c1Id, profile.PrimaryClient.Guid);
                Assert.Equal("Tst_Profile", profile.Name);
                Assert.True(profile.Accounts.LIRA);

                Assert.Equal(10, profile.Decline);
                Assert.Equal(13, profile.InitialInvestment);
                Assert.Equal(14, profile.MonthlyCommitment);
                Assert.Equal(8, ProfilePoints.RoundTimeHorizon(profile.TimeHorizon.WithdrawTime));
            }
        }

        [Fact]
        public async Task MeetingFromOriginallySingleHousehold()
        {
            DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
                .UseInMemoryDatabase(databaseName: "meetings_test_db")
                .Options;

            Meeting prevMeeting;

            Guid c1Id;
            Guid c2Id;

            // ========= SETUP ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);

                Client c1 = new Client("c_1", "c1@clone_meeting.test", DateTimeOffset.MinValue);
                Household h = new Household(c1);
                prevMeeting = h.BeginMeeting(new Advisor(Guid.NewGuid(), "", "TST", ""), 1803);

                await repo.AddAsync(prevMeeting);
                await context.SaveChangesAsync();

                Client c2 = new Client("c_2", "c2@clone_meeting.test", DateTimeOffset.MinValue);
                h.AddMember(c2);

                c1Id = c1.Guid;
                c2Id = c2.Guid;
                await context.SaveChangesAsync();
            }

            Guid newMeetingId;
            // ========= ACT / ASSERT SETUP ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);
                Meeting meeting = await repo.FindByIdAsync(prevMeeting.Guid);
                Assert.Equal(prevMeeting.Guid, meeting.Guid);

                CloneProfileFromMeetingCommand cmd = new CloneProfileFromMeetingCommand(prevMeeting.Guid);
                CloneProfileFromMeetingCommandHandler commandHandler = new CloneProfileFromMeetingCommandHandler(repo, _mockMediator.Object);

                newMeetingId = (await commandHandler.Handle(cmd, CancellationToken.None)).Data;
            }

            // ========= VERIFY =============
            await using (ProfileContext context = new ProfileContext(options))
            {
                Assert.NotEqual(prevMeeting.Guid, newMeetingId);
                Assert.NotEqual(Guid.Empty, newMeetingId);

                MeetingRepository repo = new MeetingRepository(context);
                Meeting newMeeting = await repo.FindByIdAsync(newMeetingId);

                KycDocument newDoc = newMeeting.KycDocument;

                Assert.Equal(c1Id, newDoc.PrimaryClient.Guid);
                Assert.True(newDoc.IsJoint);
                Assert.NotNull(newDoc.JointClient);
                Assert.Equal(c2Id, newDoc.JointClient.Guid);
            }
        }

        [Fact]
        public async Task MeetingFromOriginallyJointToSingleHousehold()
        {
            DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
                .UseInMemoryDatabase(databaseName: "meetings_test_db")
                .Options;

            Meeting prevMeeting;

            Guid c2Id;
            // ========= SETUP ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);

                Client c1 = new Client("c_1", "c1@clone_meeting.test", DateTimeOffset.MinValue);
                Client c2 = new Client("c_2", "c2@clone_meeting.test", DateTimeOffset.MinValue);
                Household h = new Household(c1, c2);
                prevMeeting = h.BeginMeeting(new Advisor(Guid.NewGuid(), "", "TST", ""), 1803);

                await repo.AddAsync(prevMeeting);
                await context.SaveChangesAsync();

                h.RemoveMember(c1.Guid);

                c2Id = c2.Guid;
                await context.SaveChangesAsync();
            }

            Guid newMeetingId;
            // ========= ACT / ASSERT ========
            await using (ProfileContext context = new ProfileContext(options))
            {
                MeetingRepository repo = new MeetingRepository(context);
                Meeting meeting = await repo.FindByIdAsync(prevMeeting.Guid);
                Assert.Equal(prevMeeting.Guid, meeting.Guid);

                CloneProfileFromMeetingCommand cmd = new CloneProfileFromMeetingCommand(prevMeeting.Guid);
                CloneProfileFromMeetingCommandHandler commandHandler = new CloneProfileFromMeetingCommandHandler(repo, _mockMediator.Object);

                newMeetingId = (await commandHandler.Handle(cmd, CancellationToken.None)).Data;
            }

            // ========= VERIFY =============
            await using (ProfileContext context = new ProfileContext(options))
            {
                Assert.NotEqual(prevMeeting.Guid, newMeetingId);
                Assert.NotEqual(Guid.Empty, newMeetingId);

                MeetingRepository repo = new MeetingRepository(context);
                Meeting newMeeting = await repo.FindByIdAsync(newMeetingId);

                KycDocument newDoc = newMeeting.KycDocument;

                Assert.Equal(c2Id, newDoc.PrimaryClient.Guid);
                Assert.False(newDoc.IsJoint);
            }
        }

        [Fact]
        public async Task JointClonePreservesNetWorthLink()
        {
            {
                DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
                    .UseInMemoryDatabase(databaseName: "meetings_test_db")
                    .Options;

                Meeting prevMeeting;

                Guid c1Id, newMeetingId;

                // ========= SETUP ========
                await using (ProfileContext context = new ProfileContext(options))
                {
                    MeetingRepository repo = new MeetingRepository(context);

                    Client c1 = new Client("c_1", "c1@clone_meeting.test", DateTimeOffset.MinValue);
                    Client c2 = new Client("c_2", "c2@clone_meeting.test", DateTimeOffset.MinValue);
                    Household h = new Household(c1, c2);
                    prevMeeting = h.BeginMeeting(new Advisor(Guid.NewGuid(), "", "TST", ""), 1803);

                    c1Id = c1.Guid;

                    KycDocument doc = prevMeeting.KycDocument;
                    doc.PrimaryClient.Income = new Income(500, 1, "tst_income");
                    doc.PrimaryClient.Knowledge = new Knowledge(2, "tst_knowledge");
                    // doc.PrimaryClient.NetWorth = new NetWorth(3, 2, 1, "tst_networth");

                    doc.PrimaryClient.NetWorth.LiquidAssets = 3;
                    doc.PrimaryClient.NetWorth.FixedAssets = 2;
                    doc.PrimaryClient.NetWorth.Liabilities = 1;
                    doc.PrimaryClient.NetWorth.Notes = "tst_networth";

                    await context.AddAsync(prevMeeting);
                    await context.SaveChangesAsync();

                    CloneProfileFromMeetingCommand cmd = new CloneProfileFromMeetingCommand(prevMeeting.Guid);
                    CloneProfileFromMeetingCommandHandler commandHandler = new CloneProfileFromMeetingCommandHandler(repo, _mockMediator.Object);

                    newMeetingId = (await commandHandler.Handle(cmd, CancellationToken.None)).Data;
                    Meeting newMeeting = await repo.FindByIdAsync(newMeetingId);

                    // ========= ACT ==========
                    newMeeting.KycDocument.PrimaryClient.NetWorth.LiquidAssets = 5;

                    // ======== VERIFY =======
                    Debug.Assert(newMeeting.KycDocument.JointClient != null,
                        "newMeeting.KycDocument.JointClient != null");
                    Assert.Equal(5, newMeeting.KycDocument.JointClient.NetWorth.LiquidAssets);
                }
            }
        }
    }
}