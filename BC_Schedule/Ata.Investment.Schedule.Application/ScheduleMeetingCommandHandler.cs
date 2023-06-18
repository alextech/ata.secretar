using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Cmd.Advisors;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Schedule.Cmd;

namespace Ata.Investment.Schedule.Application;

public class ScheduleMeetingCommandHandler : ICommandHandler<ScheduleMeetingCommand, CommandResponse>
{
    private readonly ProfileContext _profileContext;
    private readonly IMediator _mediatR;

    public ScheduleMeetingCommandHandler(ProfileContext profileContext, IMediator mediatR)
    {
        _profileContext = profileContext;
        _mediatR = mediatR;
    }

    public async Task<CommandResponse> Handle(ScheduleMeetingCommand scheduleMeetingCommand, CancellationToken cancellationToken)
    {
        Household household = await (
            from h in _profileContext.Households
                .TagWith("Fetching household for meeting schedule command.")
            where h.Guid == scheduleMeetingCommand.HouseholdId
            select h
        )
        .Include(h => h.PrimaryClient)
        .Include(h => h.JointClient)
        .SingleAsync(cancellationToken);

        int latestAllocationVersion = (
                await _mediatR.Send(new ListedAllocationVersionsQuery(), cancellationToken)
        ).First();

        Advisor advisor =
            await _mediatR.Send(new AdvisorByIdQuery(scheduleMeetingCommand.AdvisorGuid), cancellationToken);

        Meeting? lastMeeting = await (
            from m in _profileContext.Meetings
                   .TagWith("Select last meeting for scheduling command")
                where m.Household.Guid == household.Guid
                orderby m.Date descending
                select m
            ).FirstOrDefaultAsync(cancellationToken);

        Meeting newMeeting;
        if (lastMeeting == null)
        {
            newMeeting = household.BeginMeeting(advisor, latestAllocationVersion);
        }
        else
        {
            Guid newMeetingGuid = (
                await _mediatR.Send(new CloneProfileFromMeetingCommand(lastMeeting.Guid), cancellationToken)
            ).Data;

            newMeeting = await _mediatR.Send(new MeetingByIdQuery(newMeetingGuid), cancellationToken);
        }

        newMeeting.Date = newMeeting.KycDocument.Date = scheduleMeetingCommand.DateTime;

        await _profileContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Ok(
            $"Scheduled meeting for CLIENT NAME with {advisor.Name} on {scheduleMeetingCommand.DateTime}"
        );
    }
}