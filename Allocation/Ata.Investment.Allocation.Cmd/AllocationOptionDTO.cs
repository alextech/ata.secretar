using System.Collections.Generic;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Cmd
{
    public class AllocationOptionDTO
    {
        public string Name { get; set; }
        public int OptionNumber { get; set; }
        public List<CompositionPart> CompositionParts { get; set; } = new List<CompositionPart>();

        public static bool operator ==(AllocationOptionDTO obj1, AllocationOptionDTO obj2)
        {
            return obj1.OptionNumber == obj2.OptionNumber;
        }

        public static bool operator !=(AllocationOptionDTO obj1, AllocationOptionDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}