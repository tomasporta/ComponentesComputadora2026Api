using AutoMapper;
using ComponentesComputadoras.Application.Dtos.Cliente;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class ClienteMappingProfile : Profile
    {
        public ClienteMappingProfile()
        {
            CreateMap<ClienteRequestDto, Cliente>();
            CreateMap<Cliente, ClienteResponseDto>();

        }
    }
}
