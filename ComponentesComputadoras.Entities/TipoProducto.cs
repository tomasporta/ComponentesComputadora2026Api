using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{

    public class TipoProducto : IEntidad
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        public virtual ICollection<Producto> Productos { get; set; } = new HashSet<Producto>();
    }


}
