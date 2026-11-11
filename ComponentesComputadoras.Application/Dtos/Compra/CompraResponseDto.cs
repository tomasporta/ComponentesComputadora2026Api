using ComponentesComputadoras.Application.Dtos.CompraDetalle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.Compra
{
    public class CompraResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public List<CompraDetalleResponseDto> Detalles { get; set; }
        public decimal Total { get; set; }
    }
}
