using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Domain.KYC
{
    public class KycDocument
    {
        private List<Profile.Profile> _jointProfiles = new List<Profile.Profile>();

        public KycDocument(Guid meetingId, Advisor advisor, Client? primaryClient, int version)
        {
            MeetingId = meetingId;
            Advisor = advisor;
            PrimaryClient = new PClient(primaryClient);
            AllocationVersion = version;

            PrimaryClient.ProfileListChanged += OnProfileAddedToClient;
        }

        public KycDocument(Guid meetingId, Advisor advisor, Client primaryClient, Client jointClient, int version)
            : this(meetingId, advisor, primaryClient, version)
        {
            SwitchToJointWith(new PClient(jointClient));
        }

        public KycDocument(Guid meetingId, Advisor advisor, Household.Household household, int version)
        {
            MeetingId = meetingId;
            Advisor = advisor;
            PrimaryClient = new PClient(household.PrimaryClient);
            PrimaryClient.ProfileListChanged += OnProfileAddedToClient;
            AllocationVersion = version;

            // ReSharper disable once InvertIf
            if (household.IsJoint)
            {
                Debug.Assert(household.JointClient != null, nameof(household.JointClient) + " != null");

                SwitchToJointWith(new PClient(household.JointClient));
            }
        }
        public Guid MeetingId { get; private set; }

        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public string Purpose { get; set; } = "";

        public ServiceStandard ServiceStandard { get; set; } = ServiceStandard.WealthAccumulation;

        public Advisor Advisor { get; private set; }

        public int AllocationVersion { get; set; }

        public PClient PrimaryClient { get; private set; }

        public PClient? JointClient { get; private set; }

        public bool IsJoint => JointClient != null;

        public bool IsSingle => !IsJoint;

        public ReadOnlyCollection<Profile.Profile> JointProfiles => _jointProfiles.AsReadOnly();

        public event EventHandler ProfileListChanged;

        public void SwitchToSingleWith(PClient client)
        {
            if (JointClient?.Guid == client.Guid)
            {
                PrimaryClient.ProfileListChanged -= OnProfileAddedToClient;
                PrimaryClient = JointClient;
            }
            else
            {
                JointClient.ProfileListChanged -= OnProfileAddedToClient;
                if (PrimaryClient.Guid != client.Guid)
                {
                    throw new Exception(
                        $"Tried switching to a single client {client.Guid} which was never part of document.");
                }
            }


            JointClient = null;
            _jointProfiles.Clear();

            PrimaryClient.JointWith = null;
        }

        public void SwitchToJointWith(PClient client)
        {
            if (PrimaryClient.Guid == client.Guid)
            {
                throw new Exception(
                    $"Tried making document client {client.Guid} joint with itself.");
            }

            JointClient = client;
            JointClient.ProfileListChanged += OnProfileAddedToClient;
            _jointProfiles.Clear();

            PrimaryClient.JointWith = client;
        }

        public Dictionary<string, string> Notes { get; private set; } = new Dictionary<string, string>();

        public void AddJointProfile(Profile.Profile profile)
        {
            _jointProfiles.Add(profile);
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceAllJointProfiles(IEnumerable<Profile.Profile> profiles)
        {
            _jointProfiles.Clear();
            _jointProfiles.AddRange(profiles);
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveProfile(Profile.Profile profile)
        {
            if (PrimaryClient.RemoveProfile(profile))
            {
                return;
            }

            if (JointClient.RemoveProfile(profile))
            {
                return;
            }

            if (_jointProfiles.Remove(profile))
            {
                ProfileListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            throw new Exception("Requested non-existent profile "+ profile.Guid);
        }

        public IEnumerable<Profile.Profile> AllProfiles
        {
            get
            {
                IEnumerable<Profile.Profile> profiles = new List<Profile.Profile>();

                profiles = profiles.Concat(PrimaryClient.Profiles);

                if (IsJoint)
                {
                    Debug.Assert(JointClient != null, nameof(JointClient) + " != null");
                    profiles = profiles
                        .Concat(JointClient.Profiles)
                        .Concat(JointProfiles);
                }

                return profiles;
            }
        }

        public void SwitchAdvisor(Advisor newAdvisor)
        {
            Advisor = newAdvisor;
        }

        private void OnProfileAddedToClient(object? sender, EventArgs eventArgs)
        {
            ProfileListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum DocumentMembers
    {
        Single,
        Joint
    }
}