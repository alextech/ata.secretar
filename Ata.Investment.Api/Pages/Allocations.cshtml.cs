using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;
using Ata.Investment.Api.Hal;
using Ata.Investment.Service;

namespace Ata.Investment.Api.Pages
{
    public class Allocations : PageModel
    {
        private readonly AllocationContext _allocationContext;
        private readonly AllocationService _allocationService;

        public Allocations(AllocationService allocationService)
        {
            _allocationContext = allocationService.AllocationContext;
            _allocationService = allocationService;
        }

        public IList<VersionDTO> VersionsList { get; private set; }

        [BindProperty]
        public IList<AllocationDTO> AllocationList { get; set; } = new List<AllocationDTO>();

        [BindProperty]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        
        [BindProperty]
        public IList<string> OptionNames { get; private set; } = new List<string>();

        //TODO change to latest
        [BindProperty(SupportsGet = true)]
        public int Version { get; set; }
        
        public bool IsPublished { get; private set; }

        public async Task<RedirectToPageResult> OnGetAsync()
        {VersionDraft latestDraft = await _allocationService.FetchLatestDraftVersion();
         
            return new RedirectToPageResult(
                "Allocations",
                new
                {
                    Handler = "Version",
                    Version = latestDraft.Version
                }
            );
        }

        public async Task<IActionResult> OnGetVersionAsync()
        {
            VersionDraft versionedAllocations = await _allocationService.FetchDraftVersion(Version);

            if (versionedAllocations == null)
            {
                return new NotFoundResult();
            }

            StringReader sr = new StringReader(versionedAllocations.Draft);
            XmlSerializer serializer = new XmlSerializer(typeof(List<AllocationDTO>));
            AllocationList = (List<AllocationDTO>)serializer.Deserialize(sr);

            Notes = versionedAllocations.Notes;
            
            FillColumnNames();
            
            await InitVersionList();

            IsPublished = (
                from v in VersionsList
                where v.Version == Version
                select !v.IsDraft
            ).First();

            return new PageResult();
        }

        public async Task<RedirectToPageResult> OnGetCloneAsync()
        {
            VersionDraft nextVersionDraft = await _allocationService.CloneFrom(Version);

            return new RedirectToPageResult(
                "Allocations",
                new
                {
                    Handler = "Version",
                    Version = nextVersionDraft.Version,
                    Notes = Notes
                }
            );
        }

        public async Task<RedirectToPageResult> OnPostPublishAsync()
        {
            // do not publish from form. Publish from draft. ???
            var allocationsRs =
                from a in _allocationContext.Allocations
                select a;

            var allocations = allocationsRs.ToDictionary(a => a.Name);
            var options = new List<Option>();

            foreach (AllocationOptionDTO optionDto in AllocationList.ElementAt(0).Options)
            {
                options.Add(new Option(optionDto.Name));
            }


            List<AllocationVersion> newVersions = new List<AllocationVersion>();
            foreach (AllocationDTO allocationDto in AllocationList)
            {
                List<AllocationOption> allocationOptions = new List<AllocationOption>();
                
                for (int i = 0; i < allocationDto.Options.Count; i++)
                {
                    allocationOptions.Add(
                        options[i].CreateComposition(allocationDto.Options[i].CompositionParts)
                    );
                }
                
                AllocationVersion newVersion = allocations.GetValueOrDefault(allocationDto.Name)
                    .CreateVersion(Version)
                    .WithScoreRange(allocationDto.ScoreRange)
                    .WithOptions(allocationOptions);
                
                newVersions.Add(newVersion);
            }
            
            FillColumnNames();

            await _allocationContext.AddRangeAsync(newVersions);
            await _allocationContext.SaveChangesAsync();
            
            return new RedirectToPageResult(
                "Allocations",
                new
                {
                    Handler = "Version",
                    Version = Version
                }
            );
        }

        public async Task<AcceptedResult> OnPostSaveDraftAsync()
        {
            VersionDraft versionDraft = await (
                from vd in _allocationContext.VersionDrafts
                where vd.Version == Version
                select vd
            ).FirstAsync();
            
            StringWriter sw = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(List<AllocationDTO>));

            HydrateOptionNames();
            
            serializer.Serialize(sw, AllocationList);
            versionDraft.Notes = Notes;
            versionDraft.Draft = sw.ToString();

            await _allocationContext.SaveChangesAsync();
            
            return new AcceptedResult();
        }

        private void FillColumnNames()
        {
            if (AllocationList.Count < 1)
            {
                return;
            }
            // TODO order by option number
            foreach (AllocationOptionDTO option in AllocationList[0].Options)
            {
                OptionNames.Add(option.Name);
            }
        }

        private void HydrateOptionNames()
        {
            foreach (AllocationDTO allocationDto in AllocationList)
            {
                for (int i = 0; i < allocationDto.Options.Count; i++)
                {
                    allocationDto.Options[i].Name = OptionNames[i];
                }
            }
        }

        private async Task InitVersionList()
        {

            

        }
    }
}