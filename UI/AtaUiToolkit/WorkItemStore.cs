using System.Collections.Generic;
using System.Threading.Tasks;
using Ata.DeloSled.FeatureTracker.Domain;
using Ata.Investment.Profile.Cmd;
using MediatR;
using Microsoft.JSInterop;

namespace AtaUiToolkit;

public class WorkItemStore
{
    private readonly IMediator _mediator;

    public WorkItemStore(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [JSInvokable]
    public async Task<IEnumerable<WorkItemType>> GetAll()
    {
        return await _mediator.Send(new WorkItemTypesQuery());
    }
}