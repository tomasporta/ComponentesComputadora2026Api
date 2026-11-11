using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace ComponentesComputadoras.WebApi.Configurations
{
    /// <summary>
    /// Detecta en que servidor SQL Server se puede conectar esta PC.
    /// Primero prueba el servidor de la cadena configurada (appsettings.json /
    /// appsettings.Local.json). Si no responde, prueba los nombres mas comunes:
    /// localhost, .\SQLEXPRESS y (localdb)\MSSQLLocalDB. Usa el primero que conecte.
    /// Asi el proyecto anda en otra computadora sin tener que configurar nada.
    /// </summary>
    public static class ServidorSql
    {
        private static readonly string[] ServidoresComunes =
        {
            "localhost",
            @".\SQLEXPRESS",
            @"(localdb)\MSSQLLocalDB",
        };

        public static string Detectar(string connectionStringConfigurada)
        {
            var original = new SqlConnectionStringBuilder(connectionStringConfigurada);

            var candidatos = new List<string> { original.DataSource };
            candidatos.AddRange(ServidoresComunes);
            candidatos.AddRange(InstanciasInstaladas());
            candidatos = candidatos.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var servidor in candidatos)
            {
                Console.Write($"Probando SQL Server en '{servidor}'... ");
                if (PuedeConectar(original, servidor))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ResetColor();

                    var elegido = new SqlConnectionStringBuilder(original.ConnectionString) { DataSource = servidor };
                    return elegido.ConnectionString;
                }
                Console.WriteLine("no responde");
            }

            // Ninguno respondio: se devuelve la original y el error se informa al aplicar migraciones
            return original.ConnectionString;
        }

        /// <summary>
        /// Lee del registro de Windows las instancias de SQL Server instaladas
        /// (por ejemplo MSSQLSERVER, SQLEXPRESS, SQLEXPRESS01...).
        /// </summary>
        private static IEnumerable<string> InstanciasInstaladas()
        {
            var servidores = new List<string>();
            if (!OperatingSystem.IsWindows()) return servidores;

            try
            {
                foreach (var vista in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                {
                    using var raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, vista);
                    using var clave = raiz.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
                    if (clave is null) continue;

                    foreach (var instancia in clave.GetValueNames())
                    {
                        servidores.Add(instancia.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                            ? "localhost"
                            : $@".\{instancia}");
                    }
                }
            }
            catch
            {
                // Si no se puede leer el registro, se usan solo los servidores comunes
            }

            return servidores;
        }

        private static bool PuedeConectar(SqlConnectionStringBuilder original, string servidor)
        {
            // Se prueba contra "master" porque la base del proyecto puede no existir todavia
            var prueba = new SqlConnectionStringBuilder(original.ConnectionString)
            {
                DataSource = servidor,
                InitialCatalog = "master",
                // LocalDB puede tardar en arrancar la primera vez
                ConnectTimeout = servidor.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase) ? 30 : 5,
                Pooling = false,
            };

            try
            {
                using var conexion = new SqlConnection(prueba.ConnectionString);
                conexion.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
