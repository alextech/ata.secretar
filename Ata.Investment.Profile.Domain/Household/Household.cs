using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedKernel;

namespace Ata.Investment.Profile.Domain.Household
{
    public class Household : Entity
    {
        internal readonly List<Meeting> _meetings = new List<Meeting>();

        protected Household()
        {
            
        }

        public Household(Client primaryClient)
        {
            PrimaryClient = primaryClient;
        }

        public Household(Client primaryClient, Client jointClient) :
            this(primaryClient)
        {
            AddMember(jointClient);
        }

        public void AddMember(Client client)
        {
            if (JointClient != null)
            {
                throw new InvalidClientAdditionException();
            }

            JointClient = client;
        }

        public void RemoveMember(Guid clentId)
        {
            if (PrimaryClient.Guid == clentId)
            {
                RemoveMember(PrimaryClient);
                return;
            }

            if(JointClient?.Guid == clentId)
            {
                RemoveMember(JointClient);
                return;
            }

            throw new InvalidClientRemovalException();
        }

        public void RemoveMember(Client client)
        {
            if (PrimaryClient.Guid != client.Guid && JointClient?.Guid != client.Guid)
            {
                throw new InvalidClientRemovalException();
            }

            if (JointClient.Equals(client))
            {
                JointClient = null;
                return;
            }

            if (PrimaryClient.Equals(client))
            {
                PrimaryClient = JointClient;
                JointClient = null;
            }
        }

        public virtual IReadOnlyCollection<Meeting> Meetings => _meetings.AsReadOnly();

        public Meeting BeginMeeting(Advisor advisor, int version)
        {
            Meeting meeting = new Meeting(this, advisor, version)
            {
                Date = DateTimeOffset.UtcNow,
                Purpose = "",
                CreatedFor = $"{PrimaryClient.Name}{(IsJoint ? " and " + JointClient.Name : "")}"
            };

            _meetings.Add(meeting);

            meeting.Begin();

            return meeting;
        }

        public void Archive()
        {
            IsActive = false;
        }

        public void Restore()
        {
            IsActive = true;
        }

        [JsonProperty]
        public virtual Client PrimaryClient { get; private set; }

        [JsonProperty]
        public virtual Client? JointClient { get; private set; }

        [JsonProperty]
        public bool IsActive { get; private set; } = true;

        [JsonIgnore]
        public bool IsJoint => JointClient != null;
    }
}