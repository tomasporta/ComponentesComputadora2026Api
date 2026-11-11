using ComponentesComputadoras.Enums;
using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.SocioNegocio
{
    public class SocioNegocioRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public TipoSocio Tipo { get; set; }

        // Solo se completa el Id que corresponde al tipo elegido
        public int? ClienteId { get; set; }
        public int? ProveedorId { get; set; }
    }
}
