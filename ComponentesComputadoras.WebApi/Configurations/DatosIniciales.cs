using ComponentesComputadoras.DataAccess;
using ComponentesComputadoras.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComponentesComputadoras.WebApi.Configurations
{
    /// <summary>
    /// Carga datos de ejemplo la primera vez que se levanta la API
    /// (solo si la tabla TiposProductos esta vacia), para poder probar
    /// los endpoints apenas se baja el proyecto.
    /// </summary>
    public static class DatosIniciales
    {
        public static async Task CargarAsync(DbDataAccess db)
        {
            if (await db.TiposProductos.AnyAsync())
                return; // ya hay datos, no se toca nada

            var motherboard = new TipoProducto { Nombre = "Motherboard", Descripcion = "Placas madre" };
            var procesador = new TipoProducto { Nombre = "Procesador", Descripcion = "CPUs" };
            var memoria = new TipoProducto { Nombre = "Memoria RAM", Descripcion = "Modulos de memoria" };

            var techWorld = new Proveedor { RazonSocial = "TechWorld SA", CUIT = "30123456789", Email = "ventas@techworld.com", Telefono = "1111-1111" };
            var compuGlobal = new Proveedor { RazonSocial = "CompuGlobal", CUIT = "30987654321", Email = "info@compuglobal.com", Telefono = "2222-2222" };

            var asus = new Producto { Nombre = "ASUS Prime B450", Codigo = "MB001", Descripcion = "Motherboard AM4", Precio = 120000, Stock = 10, TipoProducto = motherboard, Proveedor = techWorld };
            var i5 = new Producto { Nombre = "Intel Core i5", Codigo = "CPU001", Descripcion = "Procesador Intel", Precio = 200000, Stock = 15, TipoProducto = procesador, Proveedor = compuGlobal };
            var ram = new Producto { Nombre = "Kingston 8GB DDR4", Codigo = "RAM001", Descripcion = "Memoria DDR4 8GB", Precio = 45000, Stock = 30, TipoProducto = memoria, Proveedor = techWorld };

            var juan = new Cliente { Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@email.com", DNI = "30111222", Telefono = "1133334444", Direccion = "" };
            var maria = new Cliente { Nombre = "María", Apellido = "Gómez", Email = "maria.gomez@email.com", DNI = "31222333", Telefono = "1155556666", Direccion = "" };

            db.AddRange(motherboard, procesador, memoria, techWorld, compuGlobal, asus, i5, ram, juan, maria);

            var compra1 = new Compra { Fecha = new DateTime(2026, 6, 1), Proveedor = techWorld };
            compra1.Detalles.Add(new CompraDetalle { Producto = asus, Cantidad = 5, PrecioUnitario = 120000 });
            var compra2 = new Compra { Fecha = new DateTime(2026, 6, 2), Proveedor = compuGlobal };
            compra2.Detalles.Add(new CompraDetalle { Producto = i5, Cantidad = 3, PrecioUnitario = 200000 });

            var venta1 = new Venta { Fecha = new DateTime(2026, 6, 3), Cliente = juan };
            venta1.Detalles.Add(new VentaDetalle { Producto = asus, Cantidad = 1, PrecioUnitario = 120000 });
            venta1.Detalles.Add(new VentaDetalle { Producto = ram, Cantidad = 2, PrecioUnitario = 45000 });
            var venta2 = new Venta { Fecha = new DateTime(2026, 6, 3), Cliente = maria };
            venta2.Detalles.Add(new VentaDetalle { Producto = i5, Cantidad = 1, PrecioUnitario = 200000 });

            db.AddRange(compra1, compra2, venta1, venta2);

            db.SociosNegocio.AddRange(
                new SocioNegocio { Nombre = "Juan Pérez", Tipo = "Cliente", Cliente = juan },
                new SocioNegocio { Nombre = "TechWorld SA", Tipo = "Proveedor", Proveedor = techWorld });

            await db.SaveChangesAsync();
        }
    }
}
