using Microsoft.AspNetCore.Components;

namespace AtaUiToolkit
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}