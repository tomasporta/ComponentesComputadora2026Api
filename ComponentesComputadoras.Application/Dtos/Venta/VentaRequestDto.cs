using ComponentesComputadoras.Application.Dtos.VentaDetalle;
using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Venta
{
    public class VentaRequestDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Elegí un cliente")]
        public int ClienteId { get; set; }

        [MinLength(1, ErrorMessage = "La venta debe tener al menos un producto")]
        public List<VentaDetalleRequestDto> Detalles { get; set; } = new();
    }
}
