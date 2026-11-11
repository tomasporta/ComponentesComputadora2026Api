using AutoMapper;
using ComponentesComputadoras.Application.Dtos.Proveedor;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class ProveedorMappingProfile : Profile
    {

        public ProveedorMappingProfile()
        {
            CreateMap<Proveedor, ProveedorResponseDto>();
            CreateMap<ProveedorRequestDto, Proveedor>();
        }
    }
}

