using ComponentesComputadoras.Application.Dtos.VentaDetalle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.Venta
{
    public class VentaResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public List<VentaDetalleResponseDto> Detalles { get; set; }
        public decimal Total { get; set; }
    }
}
