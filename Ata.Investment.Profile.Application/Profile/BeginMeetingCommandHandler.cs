using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Advisor = Ata.Investment.AuthCore.Advisor;

namespace Ata.Investment.Profile.Application.Profile
{
    public class BeginMeetingCommandHandler : IRequestHandler<BeginMeetingCommand, CommandResponse<Guid>>
    {
        private readonly UserManager<Advisor> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ProfileContext _profileContext;

        public BeginMeetingCommandHandler(UserManager<Advisor> userManager,  IHttpContextAccessor httpContext, ProfileContext profileContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            _profileContext = profileContext;
        }

        public async Task<CommandResponse<Guid>> Handle(BeginMeetingCommand beginMeetingCommand, CancellationToken cancellationToken)
        {
            Advisor identityAdvisor = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            Domain.Advisor advisor = new Domain.Advisor(
                Guid.Parse(identityAdvisor.Id), identityAdvisor.Name,
                identityAdvisor.Credentials, identityAdvisor.Email);

            Household household = await (
                from h in _profileContext.Households
                where h.Guid == beginMeetingCommand.HouseholdId
                select h
            )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient)
                .SingleAsync(cancellationToken);

            // TODO handle household not found
            // TODO handle version wrong
            Meeting newMeeting = household.BeginMeeting(advisor, beginMeetingCommand.AllocationVersion);

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse<Guid>.Ok(
                newMeeting.Guid,
                $"Began new meeting with {household.PrimaryClient.Name} using allocation version {beginMeetingCommand.AllocationVersion}."
            );
        }
    }
}