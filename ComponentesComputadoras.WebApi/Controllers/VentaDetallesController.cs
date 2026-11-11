using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.VentaDetalle;
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
    public class VentaDetallesController : ControllerBase
    {
        private readonly ILogger<VentaDetallesController> _logger;
        private readonly IApplication<VentaDetalle> _detalle;
        private readonly IApplication<Venta> _venta;
        private readonly IStockService _stock;
        private readonly IMapper _mapper;

        public VentaDetallesController(
            ILogger<VentaDetallesController> logger,
            IApplication<VentaDetalle> detalle,
            IApplication<Venta> venta,
            IStockService stock,
            IMapper mapper)
        {
            _logger = logger;
            _detalle = detalle;
            _venta = venta;
            _stock = stock;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var detalles = await _detalle.GetAllAsync();
            return Ok(_mapper.Map<IList<VentaDetalleResponseDto>>(detalles));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detalle = await _detalle.GetByIdAsync(id);
            if (detalle is null) return NotFound(new { error = "Detalle no encontrado" });

            return Ok(_mapper.Map<VentaDetalleResponseDto>(detalle));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VentaDetalleRequestDto detalleRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarVentaAsync(detalleRequestDto.VentaId);
            await _stock.MoverAsync(new[] { new MovimientoStock(detalleRequestDto.ProductoId, -detalleRequestDto.Cantidad) });

            var detalle = _mapper.Map<VentaDetalle>(detalleRequestDto);
            await _detalle.SaveAsync(detalle);

            var dto = _mapper.Map<VentaDetalleResponseDto>(detalle);
            return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] VentaDetalleRequestDto detalleRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var detalleBack = await _detalle.GetByIdAsync(id);
            if (detalleBack is null) return NotFound(new { error = "Detalle no encontrado" });

            await ValidarVentaAsync(detalleRequestDto.VentaId);

            // Se revierte lo que movio el detalle anterior y se aplica el nuevo
            await _stock.MoverAsync(new[]
            {
                new MovimientoStock(detalleBack.ProductoId, detalleBack.Cantidad),
                new MovimientoStock(detalleRequestDto.ProductoId, -detalleRequestDto.Cantidad)
            });

            _mapper.Map(detalleRequestDto, detalleBack);
            await _detalle.SaveAsync(detalleBack);

            return Ok(_mapper.Map<VentaDetalleResponseDto>(detalleBack));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var detalleBack = await _detalle.GetByIdAsync(id);
            if (detalleBack is null) return NotFound(new { error = "Detalle no encontrado" });

            await _stock.MoverAsync(new[] { new MovimientoStock(detalleBack.ProductoId, detalleBack.Cantidad) });
            await _detalle.DeleteAsync(detalleBack.Id);

            return NoContent();
        }

        private async Task ValidarVentaAsync(int ventaId)
        {
            if (await _venta.GetByIdAsync(ventaId) is null)
                throw new ReglaNegocioException("La venta indicada no existe.", ReglaNegocioException.NotFound);
        }
    }
}
