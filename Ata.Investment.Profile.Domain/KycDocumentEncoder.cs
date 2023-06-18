using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;
using Range = SharedKernel.Range;

namespace Ata.Investment.Profile.Domain
{
    public class KycDocumentEncoder
    {
        public static KycDocument Decode(string xmlKycDocument)
        {
            XElement? kycRoot = XDocument.Parse(xmlKycDocument).Root;


            KycDocument kycDocument = new KycDocument(
                Guid.Parse(kycRoot.Attribute("id")!.Value),
                BuildAdvisor(kycRoot),
                BuildClient(kycRoot.Element(nameof(KycDocument.PrimaryClient))),
                int.Parse(kycRoot.Element(nameof(KycDocument.AllocationVersion)).Value)
            );

            kycDocument.Date = DateTime.Parse(kycRoot.Element(nameof(KycDocument.Date)).Value);
            kycDocument.Purpose = kycRoot.Element(nameof(KycDocument.Purpose)).Value;
            kycDocument.ServiceStandard = Enum.Parse<ServiceStandard>(kycRoot.Element(nameof(KycDocument.ServiceStandard)).Value);

            HydratePClient(kycDocument.PrimaryClient, kycRoot.Element(nameof(KycDocument.PrimaryClient)));
            XElement? jointClientNode = kycRoot.Element(nameof(KycDocument.JointClient));
            if (jointClientNode != null)
            {
                PClient jointPClient = new PClient(BuildClient(jointClientNode));
                HydratePClient(jointPClient, jointClientNode);

                kycDocument.SwitchToJointWith(jointPClient);
            }

            XElement jointProfilesNode =  kycRoot.Element(nameof(KycDocument.JointProfiles));
            if(jointProfilesNode != null)
            {
                foreach (XElement jointProfileNode in jointProfilesNode.Elements())
                {
                    kycDocument.AddJointProfile(BuildProfile(jointProfileNode, kycDocument.PrimaryClient, kycDocument.JointClient));
                }
            }

            foreach (XElement noteElement in kycRoot.Element(nameof(KycDocument.Notes)).Elements())
            {
                kycDocument.Notes[noteElement.Name.LocalName] = noteElement.Value;
            }

            return kycDocument;
        }

        public static string Encode(KycDocument kycDocument)
        {
            PClient pClient = kycDocument.PrimaryClient;

            XElement xElement = new XElement("meeting", new XAttribute("id", kycDocument.MeetingId),
                new XElement(nameof(KycDocument.Date), kycDocument.Date),
                new XElement(nameof(KycDocument.Purpose), kycDocument.Purpose),
                new XElement(nameof(KycDocument.ServiceStandard), kycDocument.ServiceStandard),
                new XElement(nameof(KycDocument.Advisor), new XAttribute("id", kycDocument.Advisor.Guid),
                    new XElement(nameof(Advisor.Name), kycDocument.Advisor.Name),
                    new XElement(nameof(Advisor.Credentials), kycDocument.Advisor.Credentials),
                    new XElement(nameof(Advisor.Email), kycDocument.Advisor.Email)
                ),
                new XElement(nameof(KycDocument.AllocationVersion), kycDocument.AllocationVersion),
                EncodeClient(pClient, nameof(KycDocument.PrimaryClient)),
                kycDocument.IsJoint ? EncodeClient(kycDocument.JointClient!, nameof(KycDocument.JointClient)) : null,
                kycDocument.IsJoint ? new XElement(nameof(KycDocument.JointProfiles), EncodeProfiles(kycDocument.JointProfiles)) : null,
                new XElement(nameof(KycDocument.Notes),
                    EncodeNotes(kycDocument.Notes)
                )
            );

            StringWriter stringWriter = new StringWriter();
            xElement.Save(stringWriter);

            return stringWriter.ToString();
        }

        private static Advisor BuildAdvisor(XElement kycRoot)
        {
            XElement advisor = kycRoot.Element(nameof(KycDocument.Advisor));

            return new Advisor(
                new Guid(advisor.Attribute("id").Value),
                advisor.Element(nameof(Advisor.Name))!.Value,
                advisor.Element(nameof(Advisor.Credentials))!.Value,
                advisor.Element(nameof(Advisor.Email))!.Value
            );
        }

        private static Client BuildClient(XElement clientNode)
        {
            return Client.Create(
                clientNode.Element(nameof(Client.Name)).Value,
                clientNode.Element(nameof(Client.Email)).Value,
                DateTimeOffset.Parse(clientNode.Element(nameof(Client.DateOfBirth)).Value),
                new Guid(clientNode.Attribute("id").Value)
            );
        }

        private static void HydratePClient(PClient pClient, XElement clientNode)
        {
            pClient.FinancialSituation = parseIntOrDefault(clientNode.Element(nameof(PClient.FinancialSituation)));

            XElement knowledgeNode = clientNode.Element(nameof(PClient.Knowledge));
            Knowledge knowledge = new Knowledge(
                parseIntOrDefault(knowledgeNode.Element(nameof(Knowledge.Level))),
                knowledgeNode.Element(nameof(Knowledge.Notes)).Value
            );


            XElement incomeNode = clientNode.Element(nameof(PClient.Income));
            Income income = new Income(
                int.Parse(incomeNode.Element(nameof(Income.Amount)).Value),
                parseIntOrDefault(incomeNode.Element(nameof(Income.Stability))),
                incomeNode.Element(nameof(Income.Notes)).Value
            );

            XElement netWorthNode = clientNode.Element(nameof(PClient.NetWorth));
            NetWorth netWorth = new NetWorth(
                int.Parse(netWorthNode.Element(nameof(NetWorth.LiquidAssets)).Value),
                int.Parse(netWorthNode.Element(nameof(NetWorth.FixedAssets)).Value),
                int.Parse(netWorthNode.Element(nameof(NetWorth.Liabilities)).Value),
                netWorthNode.Element(nameof(NetWorth.Notes)).Value
            );

            pClient.Knowledge = knowledge;
            pClient.Income = income;
            pClient.NetWorth = netWorth;

            foreach (XElement profileNode in clientNode.Element(nameof(PClient.Profiles)).Elements())
            {
                pClient.AddProfile(BuildProfile(profileNode, pClient));
            }
        }

        private static Profile.Profile BuildProfile(XElement profileNode, PClient pClient, PClient? jointPClient = null)
        {
            Profile.Profile profile = new Profile.Profile(pClient, jointPClient)
            {
                Guid = Guid.Parse(profileNode.Attribute("id").Value),
                Name = profileNode.Element(nameof(Profile.Profile.Name)).Value,
                Goal = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.Goal))),
                PercentageOfSavings = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.PercentageOfSavings))),
                DecisionMaking = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.DecisionMaking))),
                Decline = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.Decline))),
                LossesOrGains = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.LossesOrGains))),
                ActionOnLosses = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.ActionOnLosses))),
                HypotheticalProfile = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.HypotheticalProfile))),
                LossVsGainProfile = parseIntOrDefault(profileNode.Element(nameof(Profile.Profile.LossVsGainProfile))),
                TimeHorizon = BuildTimeHorizon(profileNode.Element(nameof(Profile.Profile.TimeHorizon))),
                InitialInvestment = int.Parse(profileNode.Element(nameof(Profile.Profile.InitialInvestment)).Value),
                MonthlyCommitment = int.Parse(profileNode.Element(nameof(Profile.Profile.MonthlyCommitment)).Value),
            };

            XElement accounts = profileNode.Element(nameof(Profile.Profile.Accounts));

            profile.Accounts.RRSP   = accounts.Element(nameof(profile.Accounts.RRSP))   != null;
            profile.Accounts.LIRA   = accounts.Element(nameof(profile.Accounts.LIRA))   != null;
            profile.Accounts.TFSA   = accounts.Element(nameof(profile.Accounts.TFSA))   != null;
            profile.Accounts.RESP   = accounts.Element(nameof(profile.Accounts.RESP))   != null;
            profile.Accounts.NonReg = accounts.Element(nameof(profile.Accounts.NonReg)) != null;
            profile.Accounts.RDSP   = accounts.Element(nameof(profile.Accounts.RDSP))   != null;
            profile.Accounts.LIF    = accounts.Element(nameof(profile.Accounts.LIF))   != null;
            profile.Accounts.RIF    = accounts.Element(nameof(profile.Accounts.RIF))   != null;

            profile.Recommendation = BuildRecommendation(profileNode.Element(nameof(profile.Recommendation)));

            profile.Meta.Icon = profileNode.Element(nameof(profile.Meta)).Element(nameof(Meta.Icon)).Value;
            profile.Meta.Color = profileNode.Element(nameof(profile.Meta)).Element(nameof(Meta.Color)).Value;

            return profile;
        }

        private static TimeHorizon BuildTimeHorizon(XElement timeHorizonNode)
        {
            TimeHorizon timeHorizon = new TimeHorizon(int.Parse(timeHorizonNode.Attribute("origin").Value));
            XElement rangeNode = timeHorizonNode.Element(nameof(TimeHorizon.Range));
            Range range = new Range(
                int.Parse(rangeNode.Element(nameof(Range.Min)).Value),
                int.Parse(rangeNode.Element(nameof(Range.Max)).Value)
            );
            timeHorizon.Range = range;

            timeHorizon.WithdrawTime = int.Parse(timeHorizonNode.Element(nameof(TimeHorizon.WithdrawTime)).Value);

            return timeHorizon;
        }

        private static Recommendation? BuildRecommendation(XElement? recommendationNode)
        {
            if (recommendationNode == null)
            {
                return null;
            }
            Dictionary<string, int> composition =
                recommendationNode.Elements(nameof(Recommendation.Composition)).Elements().
                    ToDictionary(compositionPart =>
                        compositionPart.Name.LocalName,
                        compositionPart => int.Parse(compositionPart.Attribute("percent").Value)
                    );

            return new Recommendation(
                recommendationNode.Element(nameof(Recommendation.Allocation)).Value,
                recommendationNode.Element("AllocationOption").Value,
                composition,
                int.Parse(recommendationNode.Element("AllocationOption").Attribute("id").Value)
            );
        }

        private static XElement EncodeClient(PClient pClient, string clientBranchName)
        {
            return new XElement(clientBranchName, new XAttribute("id", pClient.Guid),
                new XElement(nameof(PClient.Name), pClient.Name),
                new XElement(nameof(PClient.Email), pClient.Email),
                new XElement(nameof(PClient.DateOfBirth), pClient.DateOfBirth),
                new XElement(nameof(PClient.Knowledge),
                    new XElement(nameof(Knowledge.Level),
                        pClient.Knowledge.Level == -1 ? null : pClient.Knowledge.Level),
                    new XElement(nameof(Knowledge.Notes), pClient.Knowledge.Notes)
                ),
                new XElement(nameof(PClient.Income),
                    new XElement(nameof(Income.Amount), pClient.Income.Amount),
                    new XElement(nameof(Income.Stability),
                        pClient.Income.Stability == -1 ? null : pClient.Income.Stability),
                    new XElement(nameof(Income.Notes), pClient.Income.Notes)
                ),
                new XElement(nameof(PClient.NetWorth),
                    new XElement(nameof(NetWorth.LiquidAssets), pClient.NetWorth.LiquidAssets),
                    new XElement(nameof(NetWorth.FixedAssets), pClient.NetWorth.FixedAssets),
                    new XElement(nameof(NetWorth.Liabilities), pClient.NetWorth.Liabilities),
                    new XElement(nameof(NetWorth.Notes), pClient.NetWorth.Notes)
                ),
                new XElement(nameof(PClient.FinancialSituation),
                    pClient.FinancialSituation == -1 ? null : pClient.FinancialSituation),
                new XElement(nameof(PClient.Profiles),
                    EncodeProfiles(pClient.Profiles)
                )
            );
        }

        private static IEnumerable<XElement> EncodeProfiles(IEnumerable<Profile.Profile> profiles)
        {
            List<XElement> profileNodes = new List<XElement>();
            foreach (Profile.Profile profile in profiles)
            {
                profileNodes.Add(new XElement("Profile", new XAttribute("id", profile.Guid),
                    new XElement(nameof(Profile.Profile.Name), profile.Name),
                    new XElement(nameof(Profile.Profile.Goal), profile.Goal == -1 ? null : profile.Goal),
                    new XElement(nameof(Profile.Profile.PercentageOfSavings), profile.PercentageOfSavings == -1 ? null : profile.PercentageOfSavings),
                    new XElement(nameof(Profile.Profile.DecisionMaking), profile.DecisionMaking == -1 ? null : profile.DecisionMaking),
                    new XElement(nameof(Profile.Profile.Decline), profile.Decline == -1 ? null : profile.Decline),
                    new XElement(nameof(Profile.Profile.LossesOrGains), profile.LossesOrGains == -1 ? null : profile.LossesOrGains),
                    new XElement(nameof(Profile.Profile.ActionOnLosses), profile.ActionOnLosses == -1 ? null : profile.ActionOnLosses),
                    new XElement(nameof(Profile.Profile.HypotheticalProfile), profile.HypotheticalProfile == -1 ? null : profile.HypotheticalProfile),
                    new XElement(nameof(Profile.Profile.LossVsGainProfile), profile.LossVsGainProfile == -1 ? null : profile.LossVsGainProfile),
                    new XElement(nameof(Profile.Profile.TimeHorizon), new XAttribute("origin", profile.TimeHorizon.Origin),
                        new XElement(nameof(TimeHorizon.WithdrawTime), profile.TimeHorizon.WithdrawTime),
                        new XElement(nameof(TimeHorizon.Range),
                            new XElement(nameof(Range.Min), profile.TimeHorizon.Range.Min),
                            new XElement(nameof(Range.Max), profile.TimeHorizon.Range.Max)
                        )
                    ),
                    new XElement(nameof(Profile.Profile.Accounts), EncodeProfileAccounts(profile.Accounts)),
                    new XElement(nameof(Profile.Profile.InitialInvestment), profile.InitialInvestment),
                    new XElement(nameof(Profile.Profile.MonthlyCommitment), profile.MonthlyCommitment),

                    profile.Recommendation == null ? null :
                    new XElement(nameof(Profile.Profile.Recommendation),
                        new XElement(nameof(Recommendation.Allocation), profile.Recommendation.Allocation),
                        new XElement("AllocationOption", new XAttribute("id", profile.Recommendation.AllocationOptionId), profile.Recommendation.Name),
                        new XElement(nameof(Recommendation.Composition), EncodeRecommendationComposition(profile.Recommendation))
                    ),

                    new XElement(nameof(Profile.Profile.Meta),
                        new XElement(nameof(Meta.Icon), profile.Meta.Icon),
                        new XElement(nameof(Meta.Color), profile.Meta.Color)
                    )
                ));
            }

            return profileNodes;
        }

        private static IEnumerable<XElement> EncodeProfileAccounts(Accounts accounts)
        {
            List<XElement> accountNodes = new List<XElement>();

            if (accounts.RRSP)
            {
                accountNodes.Add(new XElement(nameof(Accounts.RRSP)));
            }
            if (accounts.LIRA)
            {
                accountNodes.Add(new XElement(nameof(Accounts.LIRA)));
            }
            if (accounts.TFSA)
            {
                accountNodes.Add(new XElement(nameof(Accounts.TFSA)));
            }
            if (accounts.RESP)
            {
                accountNodes.Add(new XElement(nameof(Accounts.RESP)));
            }
            if (accounts.NonReg)
            {
                accountNodes.Add(new XElement(nameof(Accounts.NonReg)));
            }
            if (accounts.RDSP)
            {
                accountNodes.Add(new XElement(nameof(Accounts.RDSP)));
            }
            if (accounts.LIF)
            {
                accountNodes.Add(new XElement(nameof(Accounts.LIF)));
            }
            if (accounts.RIF)
            {
                accountNodes.Add(new XElement(nameof(Accounts.RIF)));
            }

            return accountNodes;
        }

        private static IEnumerable<XElement> EncodeRecommendationComposition(Recommendation recommendation)
        {
            return recommendation.Composition.Select(composition =>
                new XElement(composition.Key, new XAttribute("percent", composition.Value))).ToList();
        }

        private static IEnumerable<XElement> EncodeNotes(Dictionary<string, string> notes)
        {
            IList<XElement> noteNodes = new List<XElement>();

            foreach (KeyValuePair<string,string> note in notes)
            {
                noteNodes.Add(new XElement(note.Key, note.Value));
            }

            return noteNodes;
        }

        private static int parseIntOrDefault(XElement node)
        {
            string value = node.Value;
            return string.IsNullOrEmpty(value) ? -1 : int.Parse(value);
        }
    }
}