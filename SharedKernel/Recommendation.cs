using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SharedKernel
{
    public class Recommendation
    {
        public Recommendation(string allocation, string name, Dictionary<string, int> composition, int allocationOptionId)
        {
            Allocation = allocation;
            Name = name;
            Composition = new ReadOnlyDictionary<string, int>(composition);
            AllocationOptionId = allocationOptionId;
        }

        public string Name { get; }

        public int AllocationOptionId { get; }

        public string Allocation { get; }

        public ReadOnlyDictionary<string, int> Composition { get; }
    }
}