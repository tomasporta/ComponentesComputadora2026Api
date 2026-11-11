using ComponentesComputadoras.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Abstractions
{
    public interface ITokensParameters
    {
        string UserName { get; set; }
        string Email { get; set; }
        string PasswordHash { get; set; }
        string Id { get; set; }
        public IList<string> Roles { get; set; }
    }

}
