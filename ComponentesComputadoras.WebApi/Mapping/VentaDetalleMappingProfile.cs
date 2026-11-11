using AutoMapper;
using ComponentesComputadoras.Application.Dtos.VentaDetalle;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class VentaDetalleMappingProfile : Profile
    {
        public VentaDetalleMappingProfile()
        {
            CreateMap<VentaDetalle, VentaDetalleResponseDto>()
          .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
          .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.Cantidad * src.PrecioUnitario));

            CreateMap<VentaDetalleRequestDto, VentaDetalle>();
        }
    }
}
