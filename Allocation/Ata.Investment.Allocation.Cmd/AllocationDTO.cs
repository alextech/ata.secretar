using System.Collections.Generic;

namespace Ata.Investment.Allocation.Cmd
{
    public class AllocationDTO
    {
        public string Name { get; set; }
        public int Version { get; set; }
        public int RiskLevel { get; set; }

        public List<AllocationOptionDTO> Options { get; set; } = new List<AllocationOptionDTO>();
    }
}