using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.Venta;
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
    public class VentasController : ControllerBase
    {
        private readonly ILogger<VentasController> _logger;
        private readonly IApplication<Venta> _venta;
        private readonly IApplication<VentaDetalle> _detalle;
        private readonly IApplication<Cliente> _cliente;
        private readonly IStockService _stock;
        private readonly IMapper _mapper;

        public VentasController(
            ILogger<VentasController> logger,
            IApplication<Venta> venta,
            IApplication<VentaDetalle> detalle,
            IApplication<Cliente> cliente,
            IStockService stock,
            IMapper mapper)
        {
            _logger = logger;
            _venta = venta;
            _detalle = detalle;
            _cliente = cliente;
            _stock = stock;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var ventas = await _venta.GetAllAsync();
            return Ok(_mapper.Map<IList<VentaResponseDto>>(ventas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venta = await _venta.GetByIdAsync(id);
            if (venta is null) return NotFound(new { error = "Venta no encontrada" });

            return Ok(_mapper.Map<VentaResponseDto>(venta));
        }

        // Alta: descuenta el stock de cada producto
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VentaRequestDto ventaRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarClienteAsync(ventaRequestDto.ClienteId);
            await _stock.MoverAsync(ventaRequestDto.Detalles
                .Select(d => new MovimientoStock(d.ProductoId, -d.Cantidad)));

            var venta = _mapper.Map<Venta>(ventaRequestDto);
            await _venta.SaveAsync(venta);

            var dto = _mapper.Map<VentaResponseDto>(venta);
            return CreatedAtAction(nameof(GetById), new { id = venta.Id }, dto);
        }

        // Edicion: se revierte el stock de los detalles anteriores, se aplican los nuevos
        // y los detalles se reemplazan por los que llegan
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] VentaRequestDto ventaRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ventaBack = await _venta.GetByIdAsync(id);
            if (ventaBack is null) return NotFound(new { error = "Venta no encontrada" });

            await ValidarClienteAsync(ventaRequestDto.ClienteId);

            var anteriores = ventaBack.Detalles.ToList();
            var revertir = anteriores.Select(d => new MovimientoStock(d.ProductoId, d.Cantidad));
            var aplicar = ventaRequestDto.Detalles.Select(d => new MovimientoStock(d.ProductoId, -d.Cantidad));
            await _stock.MoverAsync(revertir.Concat(aplicar).ToList());

            foreach (var detalle in anteriores)
                await _detalle.DeleteAsync(detalle.Id);
            ventaBack.Detalles.Clear();

            ventaBack.Fecha = ventaRequestDto.Fecha;
            ventaBack.ClienteId = ventaRequestDto.ClienteId;
            foreach (var detalleDto in ventaRequestDto.Detalles)
                ventaBack.Detalles.Add(_mapper.Map<VentaDetalle>(detalleDto));

            await _venta.SaveAsync(ventaBack);

            return Ok(_mapper.Map<VentaResponseDto>(ventaBack));
        }

        // Baja: se revierte el stock y se borran los detalles junto con la venta
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var ventaBack = await _venta.GetByIdAsync(id);
            if (ventaBack is null) return NotFound(new { error = "Venta no encontrada" });

            var detalles = ventaBack.Detalles.ToList();
            await _stock.MoverAsync(detalles
                .Select(d => new MovimientoStock(d.ProductoId, d.Cantidad)).ToList());

            foreach (var detalle in detalles)
                await _detalle.DeleteAsync(detalle.Id);
            await _venta.DeleteAsync(ventaBack.Id);

            return NoContent();
        }

        private async Task ValidarClienteAsync(int clienteId)
        {
            if (await _cliente.GetByIdAsync(clienteId) is null)
                throw new ReglaNegocioException("El cliente no existe.", ReglaNegocioException.NotFound);
        }
    }
}
