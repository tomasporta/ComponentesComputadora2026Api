using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.CompraDetalle
{
    public class CompraDetalleRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Elegí un producto")]
        public int ProductoId { get; set; }

        // Dentro de una compra nueva viaja en 0; al cargar un detalle suelto es obligatorio
        public int CompraId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Range(0.01, 999999999, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }
    }
}
