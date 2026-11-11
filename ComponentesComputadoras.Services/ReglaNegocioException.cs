namespace ComponentesComputadoras.Services
{
    /// <summary>
    /// Error de negocio esperado (producto inexistente, stock insuficiente, etc.).
    /// El ManejadorDeErrores de la WebApi la convierte en una respuesta con su
    /// codigo HTTP y el mensaje en { "error": "..." }, en lugar de un 500.
    /// </summary>
    public class ReglaNegocioException : Exception
    {
        public const int BadRequest = 400;
        public const int NotFound = 404;
        public const int Conflict = 409;

        public int StatusCode { get; }

        public ReglaNegocioException(string mensaje, int statusCode = BadRequest)
            : base(mensaje)
        {
            StatusCode = statusCode;
        }
    }
}
