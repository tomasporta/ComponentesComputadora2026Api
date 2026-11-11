using ComponentesComputadoras.Application;
using ComponentesComputadoras.Entities;

namespace ComponentesComputadoras.Services.StockServices
{
    /// <summary>
    /// Un movimiento de stock: Cantidad positiva suma (compra), negativa resta (venta).
    /// </summary>
    public record MovimientoStock(int ProductoId, int Cantidad);

    public interface IStockService
    {
        /// <summary>
        /// Valida que los productos existan y que ninguno quede con stock negativo,
        /// y recien ahi aplica los movimientos sobre los productos.
        /// No guarda: los cambios se guardan junto con la compra/venta.
        /// Si algo no se cumple lanza ReglaNegocioException y no modifica nada.
        /// </summary>
        Task MoverAsync(IEnumerable<MovimientoStock> movimientos);
    }

    public class StockService : IStockService
    {
        private readonly IApplication<Producto> _producto;

        public StockService(IApplication<Producto> producto)
        {
            _producto = producto;
        }

        public async Task MoverAsync(IEnumerable<MovimientoStock> movimientos)
        {
            // Si el mismo producto aparece varias veces se suma todo en un solo cambio
            var cambios = movimientos
                .GroupBy(m => m.ProductoId)
                .Select(g => new MovimientoStock(g.Key, g.Sum(m => m.Cantidad)))
                .ToList();

            // 1) Validar todo
            var productos = new List<Producto>();
            foreach (var cambio in cambios)
            {
                var producto = await _producto.GetByIdAsync(cambio.ProductoId);
                if (producto is null)
                    throw new ReglaNegocioException(
                        $"El producto con Id {cambio.ProductoId} no existe.", ReglaNegocioException.NotFound);

                if (producto.Stock + cambio.Cantidad < 0)
                    throw new ReglaNegocioException(
                        $"Stock insuficiente para \"{producto.Nombre}\": hay {producto.Stock} y se necesitan {-cambio.Cantidad}.",
                        ReglaNegocioException.Conflict);

                productos.Add(producto);
            }

            // 2) Aplicar
            for (int i = 0; i < cambios.Count; i++)
                productos[i].Stock += cambios[i].Cantidad;
        }
    }
}
