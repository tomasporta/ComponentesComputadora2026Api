using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.TipoProducto;
using ComponentesComputadoras.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoProductosController : ControllerBase
    {
        private readonly ILogger<TipoProductosController> _logger;
        private readonly IApplication<TipoProducto> _tipoProducto;
        private readonly IMapper _mapper;

        public TipoProductosController(
            ILogger<TipoProductosController> logger,
            IApplication<TipoProducto> tipoProducto,
            IMapper mapper)
        {
            _logger = logger;
            _tipoProducto = tipoProducto;
            _mapper = mapper;
        }

        //  Obtener todos los tipos de producto
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> All()
        {
            var tipos = await _tipoProducto.GetAllAsync();
            var dto = _mapper.Map<IList<TipoProductoResponseDto>>(tipos);
            return Ok(dto);
        }

        //  Obtener tipo de producto por Id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _tipoProducto.GetByIdAsync(id);
            if (tipo is null) return NotFound();

            var dto = _mapper.Map<TipoProductoResponseDto>(tipo);
            return Ok(dto);
        }

        //  Crear tipo de producto
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] TipoProductoRequestDto tipoProductoRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var tipo = _mapper.Map<TipoProducto>(tipoProductoRequestDto);
            await _tipoProducto.SaveAsync(tipo);

            var dto = _mapper.Map<TipoProductoResponseDto>(tipo);
            return CreatedAtAction(nameof(GetById), new { id = tipo.Id }, dto);
        }

        //  Borrar tipo de producto
        //  Editar tipo de producto
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Editar(int id, [FromBody] TipoProductoRequestDto tipoProductoRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var tipo = await _tipoProducto.GetByIdAsync(id);
            if (tipo is null) return NotFound();

            _mapper.Map(tipoProductoRequestDto, tipo);
            await _tipoProducto.SaveAsync(tipo);

            return Ok(_mapper.Map<TipoProductoResponseDto>(tipo));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Borrar(int id)
        {
            var tipoBack = await _tipoProducto.GetByIdAsync(id);
            if (tipoBack is null) return NotFound();

            await _tipoProducto.DeleteAsync(tipoBack.Id);
            return NoContent();
        }
    }
}
