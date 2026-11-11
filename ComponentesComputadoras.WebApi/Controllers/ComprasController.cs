using AutoMapper;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.Application.Dtos.Compra;
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
    public class ComprasController : ControllerBase
    {
        private readonly ILogger<ComprasController> _logger;
        private readonly IApplication<Compra> _compra;
        private readonly IApplication<CompraDetalle> _detalle;
        private readonly IApplication<Proveedor> _proveedor;
        private readonly IStockService _stock;
        private readonly IMapper _mapper;

        public ComprasController(
            ILogger<ComprasController> logger,
            IApplication<Compra> compra,
            IApplication<CompraDetalle> detalle,
            IApplication<Proveedor> proveedor,
            IStockService stock,
            IMapper mapper)
        {
            _logger = logger;
            _compra = compra;
            _detalle = detalle;
            _proveedor = proveedor;
            _stock = stock;
            _mapper = mapper;
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            var compras = await _compra.GetAllAsync();
            return Ok(_mapper.Map<IList<CompraResponseDto>>(compras));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var compra = await _compra.GetByIdAsync(id);
            if (compra is null) return NotFound(new { error = "Compra no encontrada" });

            return Ok(_mapper.Map<CompraResponseDto>(compra));
        }

        // Alta: suma el stock de cada producto
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CompraRequestDto compraRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await ValidarProveedorAsync(compraRequestDto.ProveedorId);
            await _stock.MoverAsync(compraRequestDto.Detalles
                .Select(d => new MovimientoStock(d.ProductoId, d.Cantidad)));

            var compra = _mapper.Map<Compra>(compraRequestDto);
            await _compra.SaveAsync(compra);

            var dto = _mapper.Map<CompraResponseDto>(compra);
            return CreatedAtAction(nameof(GetById), new { id = compra.Id }, dto);
        }

        // Edicion: se revierte el stock de los detalles anteriores, se aplican los nuevos
        // y los detalles se reemplazan por los que llegan
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CompraRequestDto compraRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var compraBack = await _compra.GetByIdAsync(id);
            if (compraBack is null) return NotFound(new { error = "Compra no encontrada" });

            await ValidarProveedorAsync(compraRequestDto.ProveedorId);

            var anteriores = compraBack.Detalles.ToList();
            var revertir = anteriores.Select(d => new MovimientoStock(d.ProductoId, -d.Cantidad));
            var aplicar = compraRequestDto.Detalles.Select(d => new MovimientoStock(d.ProductoId, d.Cantidad));
            await _stock.MoverAsync(revertir.Concat(aplicar).ToList());

            foreach (var detalle in anteriores)
                await _detalle.DeleteAsync(detalle.Id);
            compraBack.Detalles.Clear();

            compraBack.Fecha = compraRequestDto.Fecha;
            compraBack.ProveedorId = compraRequestDto.ProveedorId;
            foreach (var detalleDto in compraRequestDto.Detalles)
                compraBack.Detalles.Add(_mapper.Map<CompraDetalle>(detalleDto));

            await _compra.SaveAsync(compraBack);

            return Ok(_mapper.Map<CompraResponseDto>(compraBack));
        }

        // Baja: se revierte el stock y se borran los detalles junto con la compra
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var compraBack = await _compra.GetByIdAsync(id);
            if (compraBack is null) return NotFound(new { error = "Compra no encontrada" });

            var detalles = compraBack.Detalles.ToList();
            await _stock.MoverAsync(detalles
                .Select(d => new MovimientoStock(d.ProductoId, -d.Cantidad)).ToList());

            foreach (var detalle in detalles)
                await _detalle.DeleteAsync(detalle.Id);
            await _compra.DeleteAsync(compraBack.Id);

            return NoContent();
        }

        private async Task ValidarProveedorAsync(int proveedorId)
        {
            if (await _proveedor.GetByIdAsync(proveedorId) is null)
                throw new ReglaNegocioException("El proveedor no existe.", ReglaNegocioException.NotFound);
        }
    }
}
