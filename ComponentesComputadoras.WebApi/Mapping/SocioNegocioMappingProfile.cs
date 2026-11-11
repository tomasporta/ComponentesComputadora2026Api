using AutoMapper;
using ComponentesComputadoras.Application.Dtos.SocioNegocio;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class SocioNegocioMappingProfile : Profile
    {
        public SocioNegocioMappingProfile()
        {
            // Request -> Entidad (el enum TipoSocio se guarda como texto)
            CreateMap<SocioNegocioRequestDto, SocioNegocio>();

            // Entidad -> Response. Cliente y Proveedor usan los mapas de
            // ClienteMappingProfile y ProveedorMappingProfile.
            CreateMap<SocioNegocio, SocioNegocioResponseDto>();
        }
    }
}
