using ComponentesComputadoras.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Entities
{
    public class Producto : IEntidad
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string Codigo { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Descripcion { get; set; } = string.Empty;


        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Relaciones
        public int TipoProductoId { get; set; }
        public virtual TipoProducto TipoProducto { get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
