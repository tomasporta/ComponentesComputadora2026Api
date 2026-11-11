using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{
    public class Proveedor : IEntidad
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string RazonSocial { get; set; } = string.Empty;
        public string CUIT { get; set; }






        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public virtual ICollection<Producto> Productos { get; set; } = new HashSet<Producto>();
        public virtual ICollection<Compra> Compras { get; set; } = new HashSet<Compra>();
        public virtual ICollection<SocioNegocio> SociosNegocio { get; set; } = new HashSet<SocioNegocio>();
    }
}


