using ComponentesComputadoras.Abstractions;

namespace ComponentesComputadoras.WebApi.Configurations
{
    public class TokenParameters : ITokensParameters
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
