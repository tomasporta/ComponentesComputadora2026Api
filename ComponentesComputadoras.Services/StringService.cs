namespace ComponentesComputadoras.Services
{
    public interface IStringService
    {
        string GetString(string stn);
    }

    public class StringService : IStringService
    {
        public string GetString(string stn)
        {
            return string.Join(" ", stn, "Funciona");
        }
    }
}
