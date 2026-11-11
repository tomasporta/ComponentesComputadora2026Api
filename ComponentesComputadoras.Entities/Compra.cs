using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{

    public class Compra : IEntidad
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        public virtual ICollection<CompraDetalle> Detalles { get; set; } = new HashSet<CompraDetalle>();
    }

}
