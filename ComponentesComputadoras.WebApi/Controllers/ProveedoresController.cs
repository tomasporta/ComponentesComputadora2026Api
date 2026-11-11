using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.Proveedor;
using ComponentesComputadoras.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ILogger<ProveedoresController> _logger;
        private readonly IApplication<Proveedor> _proveedor;
        private readonly IMapper _mapper;

        public ProveedoresController(
            ILogger<ProveedoresController> logger,
            IApplication<Proveedor> proveedor,
            IMapper mapper)
        {
            _logger = logger;
            _proveedor = proveedor;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> All()
        {
            var proveedores = await _proveedor.GetAllAsync();
            var dto = _mapper.Map<IList<ProveedorResponseDto>>(proveedores);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var proveedor = await _proveedor.GetByIdAsync(id);
            if (proveedor is null) return NotFound();

            var dto = _mapper.Map<ProveedorResponseDto>(proveedor);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] ProveedorRequestDto proveedorRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var proveedor = _mapper.Map<Proveedor>(proveedorRequestDto);
            await _proveedor.SaveAsync(proveedor);

            var dto = _mapper.Map<ProveedorResponseDto>(proveedor);
            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, dto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Editar(int id, [FromBody] ProveedorRequestDto proveedorRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var proveedor = await _proveedor.GetByIdAsync(id);
            if (proveedor is null) return NotFound();

            _mapper.Map(proveedorRequestDto, proveedor);
            await _proveedor.SaveAsync(proveedor);

            return Ok(_mapper.Map<ProveedorResponseDto>(proveedor));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Borrar(int id)
        {
            var proveedorBack = await _proveedor.GetByIdAsync(id);
            if (proveedorBack is null) return NotFound();

            await _proveedor.DeleteAsync(proveedorBack.Id);
            return NoContent();
        }
    }
}
