using System.Collections.Generic;
using System.Linq;
using SharedKernel;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Domain
{
    public class Allocation : Entity
    {
        private readonly List<AllocationVersion> _history = new List<AllocationVersion>();
        public Allocation(string name, int riskLevel) => (Name, RiskLevel) = (name, riskLevel);

        private Allocation()
        {
        }

        public string Name { get; private set; }

        public int RiskLevel { get; private set; }

        public AllocationVersion Current
        {
            get
            {
                if (_history.Count == 0)
                {
                    return new AllocationVersion(Name, 0, RiskLevel);
                }
                return _history.First();
            }
        }


        public AllocationVersion AtVersion(int version)
        {
            return _history.First(a => a.Version == version);
        }

        public AllocationBuilder CreateVersion(int version)
        {
            AllocationBuilder builder = new AllocationBuilder(this, version);
            AllocationVersion allocationVersion = (AllocationVersion) builder;
            _history.Add(allocationVersion);

            return builder;
        }
    }

    public class AllocationVersion
    {
        // acts as unique ID across application
        public string Name { get; private set; }
        public int Version { get; private set; }
        public int RiskLevel { get; private set; }
        public bool IsListed { get; set; } = true;

        internal List<AllocationOption> _options = new List<AllocationOption>();
        public IEnumerable<AllocationOption> Options => _options.AsReadOnly();

        protected AllocationVersion()
        {

        }

        internal AllocationVersion(string name, int version, int riskLevel) =>
            (Name, Version, RiskLevel) =
            (name, version, riskLevel);

        #region comparison operator
//        public override bool Equals(object obj)
//        {
//            if (obj == null)
//                return false;
//
//            AllocationVersion other = obj as AllocationVersion;
//
//            return Name == other.Name && Version == other.Version;
//        }
//
//        public override int GetHashCode()
//        {
//            return Name.GetHashCode() + Version;
//        }
//
//        public static bool operator ==(AllocationVersion x, AllocationVersion y)
//        {
//            return x.Name == y.Name &&
//                    x.Version == y.Version;
//        }
//
//        public static bool operator !=(AllocationVersion x, AllocationVersion y)
//        {
//            return !(x == y);
//        }
#endregion
    }

    public class AllocationBuilder
    {
        private readonly AllocationVersion _newAllocation;

        internal AllocationBuilder(Allocation allocation, int version)
        {
            _newAllocation = new AllocationVersion(allocation.Name, version, allocation.RiskLevel);
        }

        public AllocationBuilder WithOptions(IEnumerable<AllocationOption> options)
        {
            _newAllocation._options = options.ToList();
            for (int i = 0; i < _newAllocation._options.Count; i++)
            {
                _newAllocation._options[i].Option.OptionNumber = i;
            }
            return this;
        }

        public static implicit operator AllocationVersion(AllocationBuilder allocationBuilder)
        {
            return allocationBuilder._newAllocation;
        }
    }
}