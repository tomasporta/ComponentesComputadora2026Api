using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.Proveedor
{
    public class ProveedorResponseDto
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}
