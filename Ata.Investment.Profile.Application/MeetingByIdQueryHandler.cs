using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application
{
    public class MeetingByIdQueryHandler : IRequestHandler<MeetingByIdQuery, Meeting?>
    {
        private readonly ProfileContext _profileContext;

        public MeetingByIdQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;

        }

        public async Task<Meeting?> Handle(MeetingByIdQuery meetingQuery, CancellationToken cancellationToken)
        {
            Meeting meeting = await _profileContext.Set<Meeting>()
                    .Include(m => m.Household)
                    .TagWith("Query meeting by ID.")
                .FirstOrDefaultAsync(m => m.Guid == meetingQuery.MeetingId, cancellationToken: cancellationToken)
                ;

            if (meeting == null)
            {
                return null;
            }

            // await _profileContext.Entry(meeting)
            //     .Reference(m => m.Household)
            //     .LoadAsync(cancellationToken)
            //     .;

            await _profileContext.Entry(meeting.Household)
                .Reference(h => h.PrimaryClient)
                .LoadAsync(cancellationToken);

            await _profileContext.Entry(meeting.Household)
                .Reference(h => h.JointClient)
                .LoadAsync(cancellationToken);

            return meeting;
        }
    }
}