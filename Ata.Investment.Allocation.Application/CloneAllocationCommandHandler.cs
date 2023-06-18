using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application
{
    public class CloneAllocationCommandHandler : IRequestHandler<CloneAllocationCommand, CommandResponse<VersionDraft>>
    {
        private readonly AllocationContext _allocationContext;

        public CloneAllocationCommandHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<CommandResponse<VersionDraft>> Handle(CloneAllocationCommand cloneCommand, CancellationToken cancellationToken)
        {
            VersionDraft cloneFrom = await (
                from d in _allocationContext.VersionDrafts
                where d.Version == cloneCommand.Version
                select d
            ).FirstAsync(cancellationToken);

            VersionDraft newDraft = await GenerateNextVersionDraft();
            newDraft.Description = cloneFrom.Description + "(COPY)";
            newDraft.Notes = cloneFrom.Notes;
            newDraft.Draft = cloneFrom.Draft;

            await _allocationContext.AddAsync(newDraft, cancellationToken);
            await _allocationContext.SaveChangesAsync(cancellationToken);

            return CommandResponse<VersionDraft>.Ok(
                newDraft,
                $"Allocation version {cloneCommand.Version} cloned to version {newDraft.Version}."
            );
        }

        private async Task<VersionDraft> FetchLatestDraftVersion()
        {
            VersionDraft latestDraft = await (
                // No Tracking version is useful for REST APIs where entity becomes disconnected
                // from d in _allocationContext.VersionDrafts.AsNoTracking()
                from d in _allocationContext.VersionDrafts
                orderby d.Date descending
                select d
            ).FirstAsync();

            return latestDraft;
        }

        private async Task<VersionDraft> GenerateNextVersionDraft()
        {
            VersionDraft latestDraft = await FetchLatestDraftVersion();

            DateTime currentDate = TimeProvider.Current.UtcNow;

            string format;
            if (currentDate.Year != latestDraft.Date.Year || currentDate.Month != latestDraft.Date.Month)
            {
                format = "yyMM";
            }
            else
            {
                if (currentDate.Day != latestDraft.Date.Day)
                {
                    format = "yyMMdd";
                }
                else
                {
                    if (currentDate.Hour != latestDraft.Date.Hour)
                    {
                        format = "yyMMddHH";
                    }
                    else
                    {
                        format = "yyMMddHHmm";
                    }
                }
            }

            return new VersionDraft(
                int.Parse(currentDate.ToString(format))
            )
            {
                Draft = latestDraft.Draft
            };
        }
    }
}