using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Producto
{
    public class ProductoRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código es obligatorio")]
        [MaxLength(25)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MaxLength(255)]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Elegí un tipo de producto")]
        public int TipoProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Elegí un proveedor")]
        public int ProveedorId { get; set; }
    }
}
