using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.SocioNegocio;
using ComponentesComputadoras.Entities;
using ComponentesComputadoras.Enums;
using ComponentesComputadoras.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class SocioNegocioController : ControllerBase
    {
        private readonly ILogger<SocioNegocioController> _logger;
        private readonly IApplication<SocioNegocio> _socio;
        private readonly IApplication<Cliente> _cliente;
        private readonly IApplication<Proveedor> _proveedor;
        private readonly IMapper _mapper;

        public SocioNegocioController(
            ILogger<SocioNegocioController> logger,
            IApplication<SocioNegocio> socio,
            IApplication<Cliente> cliente,
            IApplication<Proveedor> proveedor,
            IMapper mapper)
        {
            _logger = logger;
            _socio = socio;
            _cliente = cliente;
            _proveedor = proveedor;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var socios = await _socio.GetAllAsync();
            return Ok(_mapper.Map<IList<SocioNegocioResponseDto>>(socios));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var socio = await _socio.GetByIdAsync(id);
            if (socio is null) return NotFound(new { error = "Socio de negocio no encontrado" });

            return Ok(_mapper.Map<SocioNegocioResponseDto>(socio));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SocioNegocioRequestDto socioRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarSocioAsync(socioRequestDto);

            var socio = _mapper.Map<SocioNegocio>(socioRequestDto);
            await _socio.SaveAsync(socio);

            var dto = _mapper.Map<SocioNegocioResponseDto>(socio);
            return CreatedAtAction(nameof(GetById), new { id = socio.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] SocioNegocioRequestDto socioRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var socioBack = await _socio.GetByIdAsync(id);
            if (socioBack is null) return NotFound(new { error = "Socio de negocio no encontrado" });

            await ValidarSocioAsync(socioRequestDto);

            _mapper.Map(socioRequestDto, socioBack);
            await _socio.SaveAsync(socioBack);

            return Ok(_mapper.Map<SocioNegocioResponseDto>(socioBack));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var socioBack = await _socio.GetByIdAsync(id);
            if (socioBack is null) return NotFound(new { error = "Socio de negocio no encontrado" });

            await _socio.DeleteAsync(socioBack.Id);
            return NoContent();
        }

        // El tipo define cual Id es obligatorio, y ese cliente/proveedor tiene que existir
        private async Task ValidarSocioAsync(SocioNegocioRequestDto socioRequestDto)
        {
            if (socioRequestDto.Tipo == TipoSocio.Cliente)
            {
                if (socioRequestDto.ClienteId is null)
                    throw new ReglaNegocioException("Debe especificar ClienteId para tipo Cliente.");
                if (await _cliente.GetByIdAsync(socioRequestDto.ClienteId.Value) is null)
                    throw new ReglaNegocioException("El cliente no existe.", ReglaNegocioException.NotFound);
                socioRequestDto.ProveedorId = null;
            }
            else
            {
                if (socioRequestDto.ProveedorId is null)
                    throw new ReglaNegocioException("Debe especificar ProveedorId para tipo Proveedor.");
                if (await _proveedor.GetByIdAsync(socioRequestDto.ProveedorId.Value) is null)
                    throw new ReglaNegocioException("El proveedor no existe.", ReglaNegocioException.NotFound);
                socioRequestDto.ClienteId = null;
            }
        }
    }
}
