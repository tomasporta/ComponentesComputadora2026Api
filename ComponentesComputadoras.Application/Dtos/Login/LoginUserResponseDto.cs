namespace ComponentesComputadoras.Application.Dtos.Login
{
    public class LoginUserResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public bool Login { get; set; }
    }
}
