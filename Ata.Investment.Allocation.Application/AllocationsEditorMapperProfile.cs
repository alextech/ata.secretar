using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application
{
    public class AllocationsEditorMapperProfile : AutoMapper.Profile
    {
        public AllocationsEditorMapperProfile()
        {
            MapAllocationModel();
        }

        private void MapAllocationModel()
        {
            CreateMap<VersionDraft, VersionDraft>();
        }
    }
}