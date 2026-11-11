using ComponentesComputadoras.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ComponentesComputadoras.Services.AuthServices
{

    public interface ITokenHandlerService
    {
        string GenerateJwtTokens(ITokensParameters parametros);
    }

    public class TokenHandlerService : ITokenHandlerService
    {
        private readonly JwtConfig _jwtConfig;
        public TokenHandlerService(IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        public string GenerateJwtTokens(ITokensParameters parametros)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            
            var claims = new List<Claim>
        {
            new Claim("Id", parametros.Id),
            new Claim(JwtRegisteredClaimNames.Sub, parametros.Id),
            new Claim(JwtRegisteredClaimNames.Name, parametros.UserName),
            new Claim(JwtRegisteredClaimNames.Email, parametros.Email)
        };

            if (parametros.Roles != null)
            {
                foreach (var role in parametros.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }

}
