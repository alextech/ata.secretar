using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;
using TimeZoneConverter;
using Ata.Investment.Profile.Cmd.Advisors;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;

namespace Ata.Investment.Profile.Application.Profile
{
    public class CloneProfileFromMeetingCommandHandler : IRequestHandler<CloneProfileFromMeetingCommand, CommandResponse<Guid>>
    {
        private readonly IMediator _mediatR;
        private readonly MeetingRepository _meetingRepository;

        public CloneProfileFromMeetingCommandHandler(MeetingRepository meetingRepository, IMediator _mediatR)
        {
            _meetingRepository = meetingRepository;
            this._mediatR = _mediatR;
        }

        public async Task<CommandResponse<Guid>> Handle(CloneProfileFromMeetingCommand cloneCommand, CancellationToken cancellationToken)
        {
            Meeting meeting = await _meetingRepository.FindByIdAsync(cloneCommand.MeetingId);
            Advisor advisor = await _mediatR.Send(new AdvisorByIdQuery(meeting.AdvisorGuid), cancellationToken);

            Household household = meeting.Household;

            // TODO does not make sense to use old allocation versions, if new ones are introduced.
            Meeting newMeeting = household.BeginMeeting(advisor, meeting.AllocationVersion);
            newMeeting.PopulateFrom(meeting);

            if (!household.IsJoint && newMeeting.KycDocument.IsJoint)
            {
                newMeeting.KycDocument.SwitchToSingleWith(newMeeting.KycDocument.GetClientById(household.PrimaryClient.Guid));
            }

            if (household.IsJoint && !newMeeting.KycDocument.IsJoint)
            {
                newMeeting.KycDocument.SwitchToJointWith(new PClient(household.JointClient!));
            }
            // TODO change of spouse: household.IsJoint && doc.IsJoint

            newMeeting.Date = newMeeting.KycDocument.Date = TimeProvider.Current.UtcNow;
            newMeeting.Purpose = newMeeting.KycDocument.Purpose += "(COPY)";

            await _meetingRepository.FlushAsync(cancellationToken);


            TimeZoneInfo timeZoneInfo = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
            DateTime oldDate = TimeZoneInfo.ConvertTimeFromUtc(meeting.Date.DateTime, timeZoneInfo);

            return CommandResponse<Guid>.Ok(
                newMeeting.Guid,
                $"Began new meeting with {newMeeting.Household.PrimaryClient.Name} using previous meeting from {oldDate:MMMM dd, yyyy} - \"{meeting.Purpose}\"."
            );
        }
    }
}