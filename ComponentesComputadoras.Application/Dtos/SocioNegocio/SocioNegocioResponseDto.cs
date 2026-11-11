using ComponentesComputadoras.Application.Dtos.Cliente;
using ComponentesComputadoras.Application.Dtos.Proveedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.SocioNegocio
{

    public class SocioNegocioResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int? ClienteId { get; set; }
        public int? ProveedorId { get; set; }
        public ClienteResponseDto Cliente { get; set; }
        public ProveedorResponseDto Proveedor { get; set; }
    }

}
