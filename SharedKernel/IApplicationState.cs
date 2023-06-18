using System;

namespace SharedKernel;

public interface IApplicationState
{
    public Guid UserGuid { get; set; }
}

public class ApplicationState : IApplicationState
{
    public Guid UserGuid { get; set; }
}