using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{
    public class Venta : IEntidad
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<VentaDetalle> Detalles { get; set; } = new HashSet<VentaDetalle>();
    }
}
