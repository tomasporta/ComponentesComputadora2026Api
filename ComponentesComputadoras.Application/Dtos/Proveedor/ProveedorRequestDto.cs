using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Proveedor
{
    public class ProveedorRequestDto
    {
        [Required(ErrorMessage = "La razón social es obligatoria")]
        [MaxLength(150)]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener 11 números, sin guiones")]
        public string CUIT { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
    }
}
