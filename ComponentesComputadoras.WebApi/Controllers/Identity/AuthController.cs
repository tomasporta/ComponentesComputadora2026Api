using ComponentesComputadoras.Application.Dtos.Identity.User;
using ComponentesComputadoras.Application.Dtos.Login;
using ComponentesComputadoras.Entities.MicrosoftIdentity;
using ComponentesComputadoras.Services.AuthServices;
using ComponentesComputadoras.WebApi.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenHandlerService _servicioToken;

        public AuthController(
            UserManager<User> userManager
            , ILogger<AuthController> logger
            , ITokenHandlerService servicioToken)
        {
            _userManager = userManager;
            _logger = logger;
            _servicioToken = servicioToken;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UserRegistroRequestDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeUsuario = await _userManager.FindByEmailAsync(user.Email);
            if (existeUsuario != null)
                return Conflict(new { error = "Existe un usuario registrado con el mail " + user.Email + "." });

            var creado = await _userManager.CreateAsync(new User()
            {
                Email = user.Email,
                UserName = user.Email,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos
            }, user.Password);

            if (!creado.Succeeded)
                return BadRequest(new { error = string.Join(" ", creado.Errors.Select(e => e.Description)) });

            return Ok(new UserRegistroResponseDto
            {
                NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                Email = user.Email,
                UserName = user.Email
            });
        }

        // Misma logica que "register", pero sincronica: espera cada llamada con
        // GetAwaiter().GetResult() en lugar de await (bloquea el hilo mientras espera)
        [HttpPost]
        [Route("RegisterSync")]
        [AllowAnonymous]
        public IActionResult RegistrarUsuarioSincronico([FromBody] UserRegistroRequestDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeUsuario = _userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult();
            if (existeUsuario != null)
                return Conflict(new { error = "Existe un usuario registrado con el mail " + user.Email + "." });

            var creado = _userManager.CreateAsync(new User()
            {
                Email = user.Email,
                UserName = user.Email,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos
            }, user.Password).GetAwaiter().GetResult();

            if (!creado.Succeeded)
                return BadRequest(new { error = string.Join(" ", creado.Errors.Select(e => e.Description)) });

            return Ok(new UserRegistroResponseDto
            {
                NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                Email = user.Email,
                UserName = user.Email
            });
        }

        // Misma logica que "register", pero recibe un CancellationToken: si el cliente
        // cancela la peticion (cierra la pestaña, corta la conexion) se deja de procesar
        [HttpPost]
        [Route("RegisterWithToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioConToken([FromBody] UserRegistroRequestDto user, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            cancellationToken.ThrowIfCancellationRequested();
            var existeUsuario = await _userManager.FindByEmailAsync(user.Email);
            if (existeUsuario != null)
                return Conflict(new { error = "Existe un usuario registrado con el mail " + user.Email + "." });

            cancellationToken.ThrowIfCancellationRequested();
            var creado = await _userManager.CreateAsync(new User()
            {
                Email = user.Email,
                UserName = user.Email,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos
            }, user.Password);

            if (!creado.Succeeded)
                return BadRequest(new { error = string.Join(" ", creado.Errors.Select(e => e.Description)) });

            return Ok(new UserRegistroResponseDto
            {
                NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                Email = user.Email,
                UserName = user.Email
            });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDto userLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeUsuario = await _userManager.FindByEmailAsync(userLogin.Email);
            if (existeUsuario is null || !await _userManager.CheckPasswordAsync(existeUsuario, userLogin.Password))
                return Unauthorized(new { error = "Usuario o contraseña incorrecto." });

            var roles = await _userManager.GetRolesAsync(existeUsuario);
            var parametros = new TokenParameters()
            {
                Id = existeUsuario.Id.ToString(),
                PasswordHash = existeUsuario.PasswordHash ?? string.Empty,
                UserName = existeUsuario.UserName ?? string.Empty,
                Email = existeUsuario.Email ?? string.Empty,
                Roles = roles
            };

            var jwt = _servicioToken.GenerateJwtTokens(parametros);
            return Ok(new LoginUserResponseDto()
            {
                Login = true,
                Token = jwt,
                UserName = existeUsuario.UserName,
                Mail = existeUsuario.Email
            });
        }
    }
}
