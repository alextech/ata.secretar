using Ata.Investment.ClientsList.ViewModels;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.ClientsList
{
    public class ClientsListMapperProfile : AutoMapper.Profile
    {
        public ClientsListMapperProfile()
        {
            MapViewModel();
        }

        // for blazor
        private void MapViewModel()
        {
            CreateMap<Client, ClientVM>();
            CreateMap<ClientVM, Client>();
        }
    }
}