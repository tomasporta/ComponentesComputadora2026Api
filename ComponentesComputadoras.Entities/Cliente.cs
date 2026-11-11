using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Entities
{
    public class Cliente : IEntidad
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string DNI { get; set; } = null!;

        public virtual ICollection<Venta> Ventas { get; set; } = new HashSet<Venta>();
        public virtual ICollection<SocioNegocio> SociosNegocio { get; set; } = new HashSet<SocioNegocio>();
    }
}