using ComponentesComputadoras.Application.Dtos.CompraDetalle;
using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Compra
{
    public class CompraRequestDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Elegí un proveedor")]
        public int ProveedorId { get; set; }

        [MinLength(1, ErrorMessage = "La compra debe tener al menos un producto")]
        public List<CompraDetalleRequestDto> Detalles { get; set; } = new();
    }
}
