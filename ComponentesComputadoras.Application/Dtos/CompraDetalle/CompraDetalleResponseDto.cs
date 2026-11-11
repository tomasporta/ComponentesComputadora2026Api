using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.CompraDetalle
{
    public class CompraDetalleResponseDto
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}
