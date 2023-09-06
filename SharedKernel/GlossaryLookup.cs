using System;

namespace SharedKernel;

public abstract class GlossaryLookup<T> : IGlossaryLookup where T : System.Enum
{
    public T Id { get; init; }
    public string Code { get; init; }
    public string DisplayName { get; init; }
    public bool IsActive { get; init; }
    public Type IdType => typeof(T);
}