using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.CompraDetalle;
using ComponentesComputadoras.Entities;
using ComponentesComputadoras.Services;
using ComponentesComputadoras.Services.StockServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComponentesComputadoras.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CompraDetallesController : ControllerBase
    {
        private readonly ILogger<CompraDetallesController> _logger;
        private readonly IApplication<CompraDetalle> _detalle;
        private readonly IApplication<Compra> _compra;
        private readonly IStockService _stock;
        private readonly IMapper _mapper;

        public CompraDetallesController(
            ILogger<CompraDetallesController> logger,
            IApplication<CompraDetalle> detalle,
            IApplication<Compra> compra,
            IStockService stock,
            IMapper mapper)
        {
            _logger = logger;
            _detalle = detalle;
            _compra = compra;
            _stock = stock;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var detalles = await _detalle.GetAllAsync();
            return Ok(_mapper.Map<IList<CompraDetalleResponseDto>>(detalles));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detalle = await _detalle.GetByIdAsync(id);
            if (detalle is null) return NotFound(new { error = "Detalle no encontrado" });

            return Ok(_mapper.Map<CompraDetalleResponseDto>(detalle));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CompraDetalleRequestDto detalleRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarCompraAsync(detalleRequestDto.CompraId);
            await _stock.MoverAsync(new[] { new MovimientoStock(detalleRequestDto.ProductoId, detalleRequestDto.Cantidad) });

            var detalle = _mapper.Map<CompraDetalle>(detalleRequestDto);
            await _detalle.SaveAsync(detalle);

            var dto = _mapper.Map<CompraDetalleResponseDto>(detalle);
            return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CompraDetalleRequestDto detalleRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var detalleBack = await _detalle.GetByIdAsync(id);
            if (detalleBack is null) return NotFound(new { error = "Detalle no encontrado" });

            await ValidarCompraAsync(detalleRequestDto.CompraId);

            // Se revierte lo que movio el detalle anterior y se aplica el nuevo
            await _stock.MoverAsync(new[]
            {
                new MovimientoStock(detalleBack.ProductoId, -detalleBack.Cantidad),
                new MovimientoStock(detalleRequestDto.ProductoId, detalleRequestDto.Cantidad)
            });

            _mapper.Map(detalleRequestDto, detalleBack);
            await _detalle.SaveAsync(detalleBack);

            return Ok(_mapper.Map<CompraDetalleResponseDto>(detalleBack));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var detalleBack = await _detalle.GetByIdAsync(id);
            if (detalleBack is null) return NotFound(new { error = "Detalle no encontrado" });

            await _stock.MoverAsync(new[] { new MovimientoStock(detalleBack.ProductoId, -detalleBack.Cantidad) });
            await _detalle.DeleteAsync(detalleBack.Id);

            return NoContent();
        }

        private async Task ValidarCompraAsync(int compraId)
        {
            if (await _compra.GetByIdAsync(compraId) is null)
                throw new ReglaNegocioException("La compra indicada no existe.", ReglaNegocioException.NotFound);
        }
    }
}
