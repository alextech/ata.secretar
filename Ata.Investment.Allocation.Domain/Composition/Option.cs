using System.Collections.Generic;
using System.Linq;

namespace Ata.Investment.Allocation.Domain.Composition
{
    public class Option
    {
        public string Name { get; set; }
        public int OptionNumber { get; internal set; }

        private Option() { }

        public Option(string name)
        {
            Name = name;
        }

        public AllocationOption CreateComposition (Dictionary<string, int> composition)
        {
            List<CompositionPart> compositions =
                composition.Select(part => new CompositionPart(part.Key, part.Value)).ToList();

            return new AllocationOption(this, compositions);
        }

        // even though Dictionary looks more natural for creating, for data transmission typed list works better
        public AllocationOption CreateComposition(List<CompositionPart> composition)
        {
            return new AllocationOption(this, composition);
        }
    }

    public class AllocationOption
    {
        public int Id { get; private set; }
        public Option Option { get; private set; }
        private AllocationOption() { }

        //TODO make readonly
        public IEnumerable<CompositionPart> CompositionParts { get; private set; }

        internal AllocationOption(Option option, IEnumerable<CompositionPart> composition)
        {
            Option = option;
            IEnumerable<CompositionPart> compositionParts = composition.ToList();

            if (compositionParts.Select(c => c.Percent).Sum() != 100)
            {
                throw new AllocationOptionsNotTotalingHundredException();
            }

            CompositionParts = compositionParts;
        }
    }
}