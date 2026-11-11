using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.Producto
{
    public class ProductoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int TipoProductoId { get; set; }
        public int ProveedorId { get; set; }
        public string TipoProductoNombre { get; set; }
        public string ProveedorNombre { get; set; }
    }

}
