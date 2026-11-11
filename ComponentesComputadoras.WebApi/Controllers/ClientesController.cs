using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.Cliente;
using ComponentesComputadoras.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ILogger<ClientesController> _logger;
        private readonly IApplication<Cliente> _cliente;
        private readonly IMapper _mapper;

        public ClientesController(
            ILogger<ClientesController> logger,
            IApplication<Cliente> cliente,
            IMapper mapper)
        {
            _logger = logger;
            _cliente = cliente;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<IActionResult> All()
        {
            var clientes = await _cliente.GetAllAsync();
            var dto = _mapper.Map<IList<ClienteResponseDto>>(clientes);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _cliente.GetByIdAsync(id);
            if (cliente is null) return NotFound();

            var dto = _mapper.Map<ClienteResponseDto>(cliente);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] ClienteRequestDto clienteRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = _mapper.Map<Cliente>(clienteRequestDto);
            await _cliente.SaveAsync(cliente);

            var dto = _mapper.Map<ClienteResponseDto>(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, dto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Editar(int id, [FromBody] ClienteRequestDto clienteRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = await _cliente.GetByIdAsync(id);
            if (cliente is null) return NotFound();

            _mapper.Map(clienteRequestDto, cliente);
            await _cliente.SaveAsync(cliente);

            return Ok(_mapper.Map<ClienteResponseDto>(cliente));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Borrar(int id)
        {
            var clienteBack = await _cliente.GetByIdAsync(id);
            if (clienteBack is null) return NotFound();

            await _cliente.DeleteAsync(clienteBack.Id);
            return NoContent();
        }
    }
}
