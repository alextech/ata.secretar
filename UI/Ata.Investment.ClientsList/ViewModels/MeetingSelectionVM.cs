using System;

namespace Ata.Investment.ClientsList.ViewModels
{
    public record MeetingSelectionVM
    {
        public Guid MeetingGuid { get; init; }
        public bool IsComplete { get; init; }
    }
}