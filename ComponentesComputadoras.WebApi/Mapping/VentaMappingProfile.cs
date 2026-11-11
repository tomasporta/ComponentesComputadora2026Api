using AutoMapper;
using ComponentesComputadoras.Application.Dtos.Venta;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.WebApi.Mapping
{
    public class VentaMappingProfile : Profile
    {
        public VentaMappingProfile()
        {
            CreateMap<Venta, VentaResponseDto>()
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Nombre))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)));

            CreateMap<VentaRequestDto, Venta>(); ;
        }
    }
}
