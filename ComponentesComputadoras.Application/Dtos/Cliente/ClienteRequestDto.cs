using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Cliente
{
    public class ClienteRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 números")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
    }
}
