using AutoMapper;
using ComponentesComputadoras.Application.Dtos.CompraDetalle;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class CompraDetalleMappingProfile : Profile
    {
        public CompraDetalleMappingProfile()
        {
            CreateMap<CompraDetalleRequestDto, CompraDetalle>();
            CreateMap<CompraDetalle, CompraDetalleResponseDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.Cantidad * src.PrecioUnitario));
        }
    }
}
