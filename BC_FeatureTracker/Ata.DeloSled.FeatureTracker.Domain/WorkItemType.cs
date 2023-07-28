using SharedKernel;

namespace Ata.DeloSled.FeatureTracker.Domain;

public class WorkItemType : GlossaryLookup<WorkItemTypeEnum>
{
    
}

public enum WorkItemTypeEnum
{
    Bug      = 1,
    Task     = 2,
    Feature  = 3,
}
