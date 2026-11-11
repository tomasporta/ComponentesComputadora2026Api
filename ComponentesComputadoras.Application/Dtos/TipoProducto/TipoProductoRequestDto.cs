using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.TipoProducto
{
    public class TipoProductoRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MaxLength(250)]
        public string Descripcion { get; set; } = string.Empty;
    }
}
