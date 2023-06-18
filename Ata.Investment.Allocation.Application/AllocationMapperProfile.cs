using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Application
{
    public class AllocationMapperProfile : AutoMapper.Profile
    {
        public AllocationMapperProfile()
        {
            MapAllocationDTOs();
        }

        private void MapAllocationDTOs()
        {
            CreateMap<AllocationOption, AllocationOptionDTO>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.Option.Name))
                .ForMember(dest => dest.OptionNumber,
                    opt => opt.MapFrom(src => src.Option.OptionNumber));

            CreateMap<AllocationVersion, AllocationDTO>()
                .ForMember(
                    dest => dest.Options,
                    opt => opt.MapFrom(src => src.Options));
        }
    }
}