using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{
    public class VentaDetalle : IEntidad
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
