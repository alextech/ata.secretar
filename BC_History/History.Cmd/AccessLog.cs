using System;

namespace History.Cmd
{
    public class AccessLog
    {
        public DateTime TimeStamp { get; set; }
        public string User { get; set; }
        public string LogDisplayName { get; set; }
        public string Description { get; set; }
        public string SerializedCommand { get; set; }
    }
}