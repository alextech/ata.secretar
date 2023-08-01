using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ata.DeloSled.FeatureTracker.Domain;
using Ata.Investment.Profile.Cmd;
using MediatR;

namespace Ata.Investment.Profile.Application;

public class WorkItemsQueryHandler : IRequestHandler<WorkItemTypesQuery, List<WorkItemType>>
{
    public Task<List<WorkItemType>> Handle(WorkItemTypesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<WorkItemType>()
        {
            new WorkItemType { Id = WorkItemTypeEnum.Bug, Code = "Bug", DisplayName = "Неполадка" },
            new WorkItemType { Id = WorkItemTypeEnum.Task, Code = "Task", DisplayName = "Задача" },
            new WorkItemType { Id = WorkItemTypeEnum.Feature, Code = "Feature", DisplayName = "Функционал" },
        });
    }
}