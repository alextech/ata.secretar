using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ata.Investment.Api.Components.Shared
{
    public interface INavigation
    {
        ReadOnlyCollection<string> Items { get; }
    }
}