using System;
using System.Globalization;
using Moq;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Domain.Test.Profile
{
    public class ProfileTest
    {
        [Fact]
        private void IsJointTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now));
            PClient jointClient = new PClient(new Client("Test Profile 2", "jointclient@profile.test", DateTime.Now));
            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient);

            Assert.False(profile.IsJoint);

            profile = new Domain.Profile.Profile(primaryClient, jointClient);

            Assert.True(profile.IsJoint);
        }

        [Fact]
        private void NamedConstructorTest()
        {
            Client client = new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now);
            PClient primaryClient = new PClient(client);
            KycDocument kycDocument = new KycDocument(Guid.Empty, new Advisor(Guid.Empty, "", "", ""), new Domain.Household.Household(client), 0);
            kycDocument.Date = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            NewProfileVO newProfileVO = new NewProfileVO(kycDocument: kycDocument, primary: primaryClient)
            {
                Name = "New Profile",
                Accounts = {RRSP = true, TFSA = true},
                TimeHorizon = new TimeHorizon(2025)
                {
                    Range = new SharedKernel.Range(2027, 2033),
                }
            };


            Domain.Profile.Profile profile = Domain.Profile.Profile.CreateFromVO(newProfileVO);

            Assert.True(profile.Accounts.RRSP);
            Assert.True(profile.Accounts.TFSA);
            Assert.Equal(newProfileVO.Name, profile.Name);
            Assert.Equal(4, ProfilePoints.RoundTimeHorizon(profile.TimeHorizon.WithdrawTime));
        }

        [Fact]
        private void JointProfileHouseholdIncomeTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                Income = new Income(100, 8, "in_1")
            };

            PClient jointClient = new PClient(new Client("Test Profile 2", "jointclient@profile.test", DateTime.Now))
            {
                Income = new Income(50, 4, "in_2")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient, jointClient);

            Income jointAnnualIncome = profile.AnnualIncome;
            Assert.Equal(150, jointAnnualIncome.Amount);
            Assert.Equal(4, jointAnnualIncome.Stability);
            Assert.Equal("in_1<br /><br />in_2", jointAnnualIncome.Notes);
        }

        [Fact]
        private void SingleProfileHouseholdIncomeTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                Income = new Income(100, 8, "in_1")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient);
            Income singleAnnualIncome = profile.AnnualIncome;
            Assert.Equal(100, singleAnnualIncome.Amount);
            Assert.Equal(8, singleAnnualIncome.Stability);
            Assert.Equal("in_1", singleAnnualIncome.Notes);
        }

        [Fact]
        private void SingleProfileAgeTest()
        {
            Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 05, 20));
            TimeProvider.Current = timeMock.Object;

            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.ParseExact("14/04/1989", "dd/MM/yyyy", CultureInfo.InvariantCulture)))
            {
                Income = new Income(100, 8, "in_1")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient);
            Assert.Equal(30, profile.Age);
        }

        [Fact]
        private void JointProfileAgeTest()
        {
            Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 05, 20));
            TimeProvider.Current = timeMock.Object;

            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.ParseExact("14/04/1989", "dd/MM/yyyy", CultureInfo.InvariantCulture)))
            {
                Income = new Income(100, 8, "in_1")
            };
            PClient jointClient = new PClient(new Client("Test Profile 2", "jointclient@profile.test", DateTime.ParseExact("14/04/1988", "dd/MM/yyyy", CultureInfo.InvariantCulture)))
            {
                Knowledge = new Knowledge(2, "k_2")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient, jointClient);
            Assert.Equal(31, profile.Age);
        }

        [Fact]
        private void JointProfileKnowledgeTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                Knowledge = new Knowledge(1, "k_1")
            };

            PClient jointClient = new PClient(new Client("Test Profile 2", "jointclient@profile.test", DateTime.Now))
            {
                Knowledge = new Knowledge(2, "k_2")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient, jointClient);
            Knowledge jointKnowledge = profile.Knowledge.Answer;

            Assert.Equal(1, jointKnowledge.Level);
            Assert.Equal("k_1<br /><br />k_2", jointKnowledge.Notes);
        }

        [Fact]
        private void SingleProfileKnowledgeTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                Knowledge = new Knowledge(1, "k_1")
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient);
            Knowledge jointKnowledge = profile.Knowledge.Answer;

            Assert.Equal(1, jointKnowledge.Level);
            Assert.Equal("k_1", jointKnowledge.Notes);
        }

        [Fact]
        private void JointProfileSituationTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                FinancialSituation = 3
            };

            PClient jointClient = new PClient(new Client("Test Profile 2", "jointclient@profile.test", DateTime.Now))
            {
                FinancialSituation = 4
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient, jointClient);
            int situation = profile.FinancialSituation;

            Assert.Equal(3, situation);
        }

        [Fact]
        private void SingleProfileSituationTest()
        {
            PClient primaryClient = new PClient(new Client("Test Profile 1", "testprofile@profile.tst", DateTime.Now))
            {
                FinancialSituation = 4
            };

            Domain.Profile.Profile profile = new Domain.Profile.Profile(primaryClient);
            int situation = profile.FinancialSituation;

            Assert.Equal(4, situation);
        }
    }
}