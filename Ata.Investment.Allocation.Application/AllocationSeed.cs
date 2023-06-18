using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Application
{
    public class AllocationSeed
    {
        private readonly AllocationRepository _allocationRepository;

        public AllocationSeed(AllocationRepository allocationRepository)
        {
            _allocationRepository = allocationRepository;
        }

        public async Task SeedDraft()
        {
            Option option = new Option("One Fund Option");
            Option twoFundOption = new Option("Two Fund Option");

            Domain.Allocation allocation;
            AllocationVersion allocationVersion;
            List<AllocationDTO> allocationDTOs = new List<AllocationDTO>();

            IEnumerable<int> versions = await _allocationRepository.FetchPublishedVersionNumbers();
            if (versions.Any(v => v == 1803))
            {
                return;
            }

            allocation = new Domain.Allocation("Safety",1);
            allocationVersion = allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>
                {
                    option.CreateComposition(new Dictionary<string, int>
                    {
                        { "CIG4248", 100 }
                    }),
                    twoFundOption.CreateComposition(new Dictionary<string, int>
                    {
                        { "PMO205", 0 },
                        { "EDG500", 100 }
                    })
                });
            await _allocationRepository.AddAsync(allocationVersion);
            allocationDTOs.Add(Mapper.Map<AllocationDTO>(allocationVersion));

            allocation = new Domain.Allocation("Conservative Income", 2);
            allocationVersion = allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>
                {
                    option.CreateComposition(new Dictionary<string, int>
                    {
                        { "CIG4247", 100 }
                    }),
                    twoFundOption.CreateComposition(new Dictionary<string, int>
                    {
                        { "PMO205", 20 },
                        { "EDG500", 80 }
                    })
                });
            await _allocationRepository.AddAsync(allocationVersion);
            allocationDTOs.Add(Mapper.Map<AllocationDTO>(allocationVersion));

            allocation = new Domain.Allocation("Balanced", 3);
            allocationVersion = allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>
                {
                    option.CreateComposition(new Dictionary<string, int>
                    {
                        { "CIG4246", 100 }
                    }),
                    twoFundOption.CreateComposition(new Dictionary<string, int>
                    {
                        { "PMO205", 30 },
                        { "EDG500", 70 }
                    })
                });
            await _allocationRepository.AddAsync(allocationVersion);
            allocationDTOs.Add(Mapper.Map<AllocationDTO>(allocationVersion));

            allocation = new Domain.Allocation("Growth", 4);
            allocationVersion = allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>
                {
                    option.CreateComposition(new Dictionary<string, int>
                    {
                        { "CIG4245", 100 }
                    }),
                    twoFundOption.CreateComposition(new Dictionary<string, int>
                    {
                        { "PMO205", 40 },
                        { "EDG500", 60 }
                    })
                });
            await _allocationRepository.AddAsync(allocationVersion);
            allocationDTOs.Add(Mapper.Map<AllocationDTO>(allocationVersion));

            allocation = new Domain.Allocation("Aggressive Growth", 5);
            allocationVersion = allocation.CreateVersion(1803)
                .WithOptions(new List<AllocationOption>
                {
                    option.CreateComposition(new Dictionary<string, int>
                    {
                        { "CIG4244", 100 }
                    }),
                    twoFundOption.CreateComposition(new Dictionary<string, int>
                    {
                        { "PMO205", 50 },
                        { "EDG500", 50 }
                    })
                });
            await _allocationRepository.AddAsync(allocationVersion);
            allocationDTOs.Add(Mapper.Map<AllocationDTO>(allocationVersion));

            VersionDraft versionDraft = new VersionDraft(1803) {Draft = JsonSerializer.Serialize(allocationDTOs)};

            await _allocationRepository.AddAsync(versionDraft);

            await _allocationRepository.FlushAsync();
        }
    }
}