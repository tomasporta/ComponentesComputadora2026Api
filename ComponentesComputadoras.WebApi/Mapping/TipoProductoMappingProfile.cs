using AutoMapper;
using ComponentesComputadoras.Application.Dtos.TipoProducto;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class TipoProductoMappingProfile : Profile
    {
        public TipoProductoMappingProfile()
        {
            CreateMap<TipoProducto, TipoProductoResponseDto>();
            CreateMap<TipoProductoRequestDto, TipoProducto>();
        }
    }
}
