using System;

namespace KycViewer.Components.Navigation
{
    public class NewProfileSelectionEvent
    {
        public Guid ForClientId { get; init; } = Guid.Empty;
        public bool IsJoint { get; init; }
        public int From { get; init; }
        public int To { get; init; }
        public string PlaceholderId { get; init; }
    }

    public class ProfileResizeEvent
    {

    }
}