using System.Collections.Generic;
using System.Linq;
using SharedKernel;
using Ata.Investment.Allocation.Domain.Composition;
using Xunit;

namespace Ata.Investment.Allocation.Domain.Test
{
    public class AllocationTest
    {

        [Fact]
        public void CreationVersionWithOptionsTest()
        {
            Allocation allocation = new Allocation("growth", 4);
            
            /*
             * TODO convert to DTOs. Put back as dictionaries.
             * EfCore private collection would return new readonly list anyway
             * so might as well return it as dictionary.
             * Therefore, during creation can take dictionary in, and convert to DTOs during save.
             */

            allocation.CreateVersion(1809);

            Option option = new Option("Added during creation");
            AllocationOption allocationOption = option.CreateComposition(new Dictionary<string, int>(){ {"pmo500", 100}});

            allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>() {allocationOption});

            Assert.Equal("Added during creation", allocation.AtVersion(1803).Options.ElementAt(0).Option.Name);
        }
    }
}