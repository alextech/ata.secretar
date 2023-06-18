#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Cmd.Draft;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Application.Draft
{
    public class PublishDraftCommandHandler : IRequestHandler<PublishDraftCommand, CommandResponse>
    {
        private readonly AllocationContext _allocationContext;
        private readonly IMediator _mediator;

        public PublishDraftCommandHandler(AllocationContext allocationContext, IMediator mediator)
        {
            _allocationContext = allocationContext;
            _mediator = mediator;
        }

        public async Task<CommandResponse> Handle(PublishDraftCommand draftCommand, CancellationToken cancellationToken)
        {
            VersionDraft? draftVersion = await _mediator.Send(new DraftAtVersionQuery(draftCommand.Version), cancellationToken);

            if (draftVersion == null)
            {
                throw new Exception("Given non-existent draft version " + draftCommand.Version);
            }

            List<AllocationDTO> allocationDTOs = JsonSerializer.Deserialize<List<AllocationDTO>>(draftVersion.Draft);

            List<Option> options = (
                from oDTO in allocationDTOs.First().Options
                select new Option(oDTO.Name)
            ).ToList();

            foreach (AllocationDTO allocationDTO in allocationDTOs)
            {
                Domain.Allocation allocation = new Domain.Allocation(allocationDTO.Name, allocationDTO.RiskLevel);
                AllocationBuilder allocationBuilder = allocation
                    .CreateVersion(draftVersion.Version);

                List<AllocationOption> allocationOptions =
                    allocationDTO.Options.Select(optionDTO => options.ElementAt(allocationDTO.Options.IndexOf(optionDTO))
                        .CreateComposition(optionDTO.CompositionParts))
                    .ToList();

                AllocationVersion allocationVersion = allocationBuilder
                    .WithOptions(allocationOptions);
                await _allocationContext.AddAsync(allocationVersion, cancellationToken);
            }

            await _allocationContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Published allocation version {draftCommand.Version}"
            );
        }
    }
}