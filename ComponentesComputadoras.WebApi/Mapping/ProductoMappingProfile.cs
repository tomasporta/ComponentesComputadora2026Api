using AutoMapper;
using ComponentesComputadoras.Application.Dtos.Producto;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class ProductoMappingProfile : Profile
    {
        public ProductoMappingProfile()
        {
            /// De entidad a ResponseDto
            CreateMap<Producto, ProductoResponseDto>()
            .ForMember(dest => dest.TipoProductoNombre, opt => opt.MapFrom(src => src.TipoProducto.Nombre))
            .ForMember(dest => dest.ProveedorNombre, opt => opt.MapFrom(src => src.Proveedor.RazonSocial));

            CreateMap<ProductoRequestDto, Producto>();
        }
    }
}
