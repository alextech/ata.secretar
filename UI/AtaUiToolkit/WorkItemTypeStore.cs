using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ata.DeloSled.FeatureTracker.Domain;
using Ata.Investment.Profile.Cmd;
using MediatR;
using Microsoft.JSInterop;
using SharedKernel;

namespace AtaUiToolkit;

public class WorkItemTypeStore : IStore
{
    private readonly IMediator _mediator;
    private IEnumerable<WorkItemType> _workItemTypes;

    public WorkItemTypeStore(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<IEnumerable<WorkItemType>> FetchAll()
    {
        _workItemTypes = await _mediator.Send(new WorkItemTypesQuery());
        return _workItemTypes;
    }

    [JSInvokable]
    public async Task<IEnumerable<GlossaryJsDto>> FetchAllJsProxy()
    {
        await FetchAll();
        return _workItemTypes.Select(item => new GlossaryJsDto()
        {
            Id = (int)item.Id,
            DisplayName = item.DisplayName
        });
    }
    
    public WorkItemType FetchById(WorkItemTypeEnum id)
    {
        return _workItemTypes.Single(item => item.Id == id);
    }
}