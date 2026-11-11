using ComponentesComputadoras.Application.Dtos.Compra;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class CompraMappingProfile : AutoMapper.Profile
    {
        public CompraMappingProfile()
        {
            CreateMap<Compra, CompraResponseDto>()
              .ForMember(dest => dest.ProveedorNombre, opt => opt.MapFrom(src => src.Proveedor.RazonSocial))
              .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)));

            CreateMap<CompraRequestDto, Compra>();
        }
    }
}
