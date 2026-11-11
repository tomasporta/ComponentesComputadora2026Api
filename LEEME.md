# ComponentesComputadoras – Web API

API en ASP.NET Core 8 + Entity Framework Core + SQL Server, con login JWT.

## Qué necesitás instalado

- **.NET 8 SDK** (o Visual Studio 2022 con la carga "Desarrollo de ASP.NET y web").
- **SQL Server** en cualquiera de sus variantes: SQL Server (Developer/Express) o LocalDB (viene con Visual Studio).
- (Opcional) SQL Server Management Studio para mirar la base.

## Pasos para levantarlo

1. **Descomprimir / clonar** el proyecto y abrir `ComponentesComputadora.sln`.
2. **Conexión a SQL Server: en general no hace falta tocar nada.** Al arrancar, la API
   prueba sola `localhost`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB` y las instancias
   instaladas en la PC, y usa la primera que responda (lo muestra en la consola).
   - Solo si ninguna responde (o querés forzar otra), copiá `ComponentesComputadoras.WebApi/appsettings.Local.example.json`
     como `appsettings.Local.json` (misma carpeta) y poné tu cadena de conexión.
     Ese archivo tiene prioridad sobre `appsettings.json` y no se sube a Git.
3. **Ejecutar** el proyecto `ComponentesComputadoras.WebApi` (F5 en Visual Studio, o
   `dotnet run --project ComponentesComputadoras.WebApi` desde la carpeta de la solución).
4. Se abre **Swagger** en `http://localhost:5106/swagger` (o `https://localhost:7136/swagger`).

**No hace falta crear la base ni correr `Update-Database`**: al arrancar, la API crea
la base `VentaComponentes`, aplica las migraciones y carga datos de ejemplo (productos,
proveedores, clientes, compras y ventas) si está vacía.

## ¿Cuál es mi servidor?

| Tenés instalado | Server= |
|---|---|
| SQL Server (instancia por defecto) | `localhost` |
| SQL Server Express | `.\SQLEXPRESS` |
| LocalDB (Visual Studio) | `(localdb)\MSSQLLocalDB` |
| SQL Server en Docker | `localhost,1433;User Id=sa;Password=...` (sin `Trusted_Connection`) |

Tip: el nombre exacto es el que aparece en "Server name" al conectarte con SSMS.

> En JSON la barra invertida se escribe doble: `.\\SQLEXPRESS`, `(localdb)\\MSSQLLocalDB`.

## Cómo probar los endpoints

1. En Swagger, `POST /api/Auth/login` con el usuario admin que se crea solo:
   ```json
   { "email": "admin@email.com", "password": "Admin123!" }
   ```
2. Copiar el `token` de la respuesta.
3. Botón **Authorize** (arriba a la derecha) → pegar el token → Authorize.
4. Ya podés usar los endpoints protegidos (Productos, Proveedores, Ventas, etc.).

También se puede registrar un usuario nuevo con `POST /api/Auth/register`.

## Estructura de la solución

| Proyecto | Contenido |
|---|---|
| `ComponentesComputadoras.Abstractions` | Interfaces base: `IEntidad`, `IDbOperation<T>`, `IDbContext<T>`, `ITokensParameters` |
| `ComponentesComputadoras.Entities` | Entidades (Cliente, Producto, Venta…) y las de Identity (`MicrosoftIdentity/`) |
| `ComponentesComputadoras.Enums` | Enumeraciones (`TipoSocio`) |
| `ComponentesComputadoras.DataAccess` | `DbDataAccess` (EF Core), `DbContext<T>` genérico y migraciones |
| `ComponentesComputadoras.Repository` | `Repository<T>` |
| `ComponentesComputadoras.Application` | `Application<T>` y los DTOs (`Dtos/<Entidad>`, `Dtos/Identity/User`, `Dtos/Login`) |
| `ComponentesComputadoras.Services` | Servicios: JWT (`AuthServices/`), stock (`StockServices/`) |
| `ComponentesComputadoras.WebApi` | `Controllers/` (Auth en `Controllers/Identity`), `Mapping/` (AutoMapper), `Configurations/` y `Program.cs` |

Los controllers solo usan `IApplication<T>` (Controller → Application → Repository → DataAccess).

## Reglas de negocio

- **Stock:** una venta descuenta del stock la cantidad de cada producto y una compra la suma.
  Al editar o borrar una venta/compra (o uno de sus detalles) el stock se recalcula.
  Si no hay stock suficiente, la API responde `409` con el producto que falta.
- **Validaciones:** los DTOs de la capa Application validan los datos (campos obligatorios,
  CUIT de 11 dígitos, DNI de 7 u 8, cantidades y precios mayores a 0). Si algo no cumple,
  la API responde `400` con el detalle de cada campo.
- **Errores:** ningún error termina en un `500` con el stack trace. Un producto, cliente o
  proveedor inexistente devuelve `404`; borrar algo que está en uso devuelve `409`. Siempre
  con el formato `{ "error": "mensaje" }` (ver `Configurations/ManejadorDeErrores.cs`).

## Problemas comunes

- **"No se pudo conectar a SQL Server" / error 26 o 40 al arrancar** → ningún servidor respondió.
  Abrí `services.msc` y verificá que el servicio **SQL Server (...)** esté *En ejecución*
  (si está detenido, clic derecho → Iniciar). Si tu servidor tiene otro nombre, ponelo en
  `appsettings.Local.json` (paso 2).
- **Aviso de certificado HTTPS** → correr una vez `dotnet dev-certs https --trust`,
  o usar el perfil `http`.
- **Login failed for user** → tu SQL Server usa usuario/clave en vez de autenticación de
  Windows: usá la variante con `User Id` y `Password`.
