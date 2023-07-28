using System.Collections.Generic;
using Ata.DeloSled.FeatureTracker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Ata.Investment.Api.Controllers;

// [ApiController]
// [Route("[controller]")]
public class WorkItemGlossaryController : Controller
{
    // [HttpGet]
    public ActionResult<List<WorkItemType>> Get()
    {
        return new List<WorkItemType>()
        {
            new WorkItemType { Id = WorkItemTypeEnum.Bug, Code = "Bug", DisplayName = "Неполадка" },
            new WorkItemType { Id = WorkItemTypeEnum.Task, Code = "Task", DisplayName = "Задача" },
            new WorkItemType { Id = WorkItemTypeEnum.Feature, Code = "Feature", DisplayName = "Функционал" },
        };
    }
}