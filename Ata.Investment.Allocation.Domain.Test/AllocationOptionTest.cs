using System.Collections.Generic;
using System.Linq;
using Ata.Investment.Allocation.Domain.Composition;
using Xunit;

namespace Ata.Investment.Allocation.Domain.Test
{
    public class AllocationOptionTest
    {

        [Fact]
        public void DefineAndFillCompositionComponentsTest()
        {
            Allocation allocation = new Allocation("growth", 4);
            AllocationVersion allocationVersion = allocation.CreateVersion(1803);

            Option option = new Option("Two Fund Option");

            AllocationOption allocationOption = option.CreateComposition(
                new Dictionary<string, int>()
                {
                    {"PMO205", 20},
                    {"EDG500", 80}
                }
            );

            List<CompositionPart> expected = new List<CompositionPart>()
            {
                new CompositionPart("PMO205", 20),
                new CompositionPart("EDG500", 80)
            };
            Assert.True(!expected.Except(allocationOption.CompositionParts).Any() && 2 == allocationOption.CompositionParts.Count());
        }

        [Fact]
        public void ThrowsExceptionNotAddingToHundredTest()
        {
            Allocation allocation = new Allocation("growth", 4);
            AllocationVersion allocationVersion = allocation.CreateVersion(1803);

            Option option = new Option("Failed option");

            Assert.Throws<AllocationOptionsNotTotalingHundredException>(() =>
                option.CreateComposition(
                    new Dictionary<string, int>()
                    {
                        {"PMO205", 90},
                        {"EDG500", 5}
                    }
                )
            );
        }

        [Fact]
        public void CreateAllocationOptionCompositionFromListTest()
        {
            Allocation allocation = new Allocation("growth", 4);
            AllocationVersion allocationVersion = allocation.CreateVersion(1803);

            Option option = new Option("Two Fund Option");

            AllocationOption allocationOption = option.CreateComposition(
                new List<CompositionPart>()
                {
                    new CompositionPart("PMO205", 20),
                    new CompositionPart("EDG500", 80)
                }
            );

            List<CompositionPart> expected = new List<CompositionPart>()
            {
                new CompositionPart("PMO205", 20),
                new CompositionPart("EDG500", 80)
            };
            Assert.True(!expected.Except(allocationOption.CompositionParts).Any() && 2 == allocationOption.CompositionParts.Count());
        }

        [Fact]
        public void CompositionMustFollowTemplateTest()
        {

        }
    }
}
