using SharedKernel;

namespace Ata.DeloSled.FeatureTracker.Domain;

public class WorkItem : Entity
{
    public WorkItem(string title)
    {
        Title = title;
    }

    public StateEnum StateId { get; init; } // TODO сделать private set
    public IWorkItemState State { get; } = new NewState();
    public string Title { get; init; }
}

public enum StateEnum
{
    New,
    Active,
    Closed,
}


public abstract class Lookup
{
    public abstract string DisplayName { get; }
}

public interface IWorkItemState
{
    public abstract string DisplayName { get; }
}

public class NewState : Lookup, IWorkItemState
{
    public StateEnum Id => StateEnum.New;
    public override string DisplayName => "New";
}

public class ActiveState : Lookup,  IWorkItemState
{
    public StateEnum Id => StateEnum.Active;
    public override string DisplayName => "Active";
}

public class ClosedState : Lookup, IWorkItemState
{
    public StateEnum Id => StateEnum.Closed;
    public override string DisplayName => "Closed";
}