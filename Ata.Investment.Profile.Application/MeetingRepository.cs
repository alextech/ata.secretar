using System;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application
{
    public class MeetingRepository : GenericRepository<Meeting, ProfileContext>
    {
        private readonly ProfileContext _profileContext;

        public MeetingRepository(ProfileContext profileContext) : base(profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<Meeting> FindByIdAsync(Guid id)
        {
            Meeting meeting = await base.FindByIdAsync(id);

            await _profileContext.Entry(meeting)
                .Reference(m => m.Household)
                .LoadAsync();

            await _profileContext.Entry(meeting.Household)
                .Reference(h => h.PrimaryClient)
                .LoadAsync();

            await _profileContext.Entry(meeting.Household)
                .Reference(h => h.JointClient)
                .LoadAsync();

            return meeting;
        }
    }
}