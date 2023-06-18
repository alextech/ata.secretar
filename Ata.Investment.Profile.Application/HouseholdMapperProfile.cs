using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application
{
    public class HouseholdMapperProfile : AutoMapper.Profile
    {
        public HouseholdMapperProfile()
        {
            MapHouseholdDTOs();
        }

        private void MapHouseholdDTOs()
        {
            CreateMap<Client, Client>();
        }
    }
}