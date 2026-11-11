using ComponentesComputadoras.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ComponentesComputadoras.WebApi.Configurations
{
    /// <summary>
    /// Manejo global de excepciones: ninguna peticion termina en un 500 con el
    /// stack trace. Siempre se responde un JSON { "error": "...", "status": N }.
    /// </summary>
    public class ManejadorDeErrores : IExceptionHandler
    {
        private readonly ILogger<ManejadorDeErrores> _logger;

        public ManejadorDeErrores(ILogger<ManejadorDeErrores> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, mensaje) = exception switch
            {
                ReglaNegocioException regla => (regla.StatusCode, regla.Message),

                // El cliente cancelo la peticion (ej: RegisterWithToken con CancellationToken)
                OperationCanceledException => (499, "La petición fue cancelada."),

                // Restricciones de la base: borrar algo que otro registro usa (clave foranea)
                DbUpdateException when HttpMethods.IsDelete(httpContext.Request.Method) =>
                    (StatusCodes.Status409Conflict,
                    "No se puede eliminar porque está siendo usado por otros registros (productos, compras, ventas o socios)."),

                DbUpdateException => (StatusCodes.Status409Conflict,
                    "No se pudo guardar: el registro está relacionado con otros datos o tiene valores inválidos."),

                _ => (StatusCodes.Status500InternalServerError,
                    "Ocurrió un error inesperado en el servidor.")
            };

            if (status == 499 || httpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("{Path}: petición cancelada por el cliente", httpContext.Request.Path);
                return true;
            }

            if (status >= 500)
                _logger.LogError(exception, "Error no controlado en {Path}", httpContext.Request.Path);
            else
                _logger.LogWarning("{Path}: {Mensaje}", httpContext.Request.Path, mensaje);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new { error = mensaje, status }, cancellationToken);
            return true;
        }
    }
}
