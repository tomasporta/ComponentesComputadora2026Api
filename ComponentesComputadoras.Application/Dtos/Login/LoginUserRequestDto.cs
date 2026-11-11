using System.ComponentModel.DataAnnotations;

namespace ComponentesComputadoras.Application.Dtos.Login
{
    public class LoginUserRequestDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
