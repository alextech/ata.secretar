namespace Ata.Investment.Allocation.Cmd
{
    public class VersionDTO
    {
        public int Version { get; set; }
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDraft => !IsPublished;
    }
}