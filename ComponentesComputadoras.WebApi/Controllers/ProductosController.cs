using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.Producto;
using ComponentesComputadoras.Entities;
using ComponentesComputadoras.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ILogger<ProductosController> _logger;
        private readonly IApplication<Producto> _producto;
        private readonly IApplication<TipoProducto> _tipoProducto;
        private readonly IApplication<Proveedor> _proveedor;
        private readonly IMapper _mapper;

        public ProductosController(
            ILogger<ProductosController> logger,
            IApplication<Producto> producto,
            IApplication<TipoProducto> tipoProducto,
            IApplication<Proveedor> proveedor,
            IMapper mapper)
        {
            _logger = logger;
            _producto = producto;
            _tipoProducto = tipoProducto;
            _proveedor = proveedor;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var productos = await _producto.GetAllAsync();
            return Ok(_mapper.Map<IList<ProductoResponseDto>>(productos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _producto.GetByIdAsync(id);
            if (producto is null) return NotFound(new { error = "Producto no encontrado" });

            return Ok(_mapper.Map<ProductoResponseDto>(producto));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoRequestDto productoRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarRelacionesAsync(productoRequestDto);

            var producto = _mapper.Map<Producto>(productoRequestDto);
            await _producto.SaveAsync(producto);

            var dto = _mapper.Map<ProductoResponseDto>(producto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] ProductoRequestDto productoRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productoBack = await _producto.GetByIdAsync(id);
            if (productoBack is null) return NotFound(new { error = "Producto no encontrado" });

            await ValidarRelacionesAsync(productoRequestDto);

            _mapper.Map(productoRequestDto, productoBack);
            await _producto.SaveAsync(productoBack);

            return Ok(_mapper.Map<ProductoResponseDto>(productoBack));
        }

        // Si el producto esta en compras o ventas la base no deja borrarlo:
        // el ManejadorDeErrores responde 409 con el motivo
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var productoBack = await _producto.GetByIdAsync(id);
            if (productoBack is null) return NotFound(new { error = "Producto no encontrado" });

            await _producto.DeleteAsync(productoBack.Id);
            return NoContent();
        }

        private async Task ValidarRelacionesAsync(ProductoRequestDto dto)
        {
            if (await _tipoProducto.GetByIdAsync(dto.TipoProductoId) is null)
                throw new ReglaNegocioException("El tipo de producto no existe.", ReglaNegocioException.NotFound);
            if (await _proveedor.GetByIdAsync(dto.ProveedorId) is null)
                throw new ReglaNegocioException("El proveedor no existe.", ReglaNegocioException.NotFound);
        }
    }
}
