using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Application.Dtos.Cliente
{
    public class ClienteResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; } = null!;   
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}
