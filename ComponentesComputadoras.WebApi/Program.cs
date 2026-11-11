
using ComponentesComputadoras.Abstractions;
using ComponentesComputadoras.Application;
using ComponentesComputadoras.DataAccess;
using ComponentesComputadoras.Entities.MicrosoftIdentity;
using ComponentesComputadoras.Repository;
using ComponentesComputadoras.Services;
using ComponentesComputadoras.Services.AuthServices;
using ComponentesComputadoras.WebApi.Configurations;
using ComponentesComputadoras.Services.StockServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Archivo opcional para que cada persona ponga SU cadena de conexion
        // sin tocar appsettings.json (ver appsettings.Local.example.json y LEEME.md)
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

        // Controllers con soporte para enums como string
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ComponentesComputadoras.WebApi", Version = "v1" });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put your Token below",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });
        });

        // Busca automaticamente un SQL Server que responda en esta PC
        // (localhost, .\SQLEXPRESS o LocalDB) y usa el primero que encuentre.
        var connectionString = ServidorSql.Detectar(builder.Configuration.GetConnectionString("DefaultConnection")!);
        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

        // Configuración de DbContext con SQL Server
        builder.Services.AddDbContext<DbDataAccess>(options =>
        {
            options.UseSqlServer(connectionString,
                    o => o.MigrationsAssembly("ComponentesComputadoras.DataAccess"));
            options.UseLazyLoadingProxies();
        });

        // Configuración JWT
        builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwt =>
        {
            var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
            jwt.SaveToken = true;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
        });

        
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<DbDataAccess>()
        .AddDefaultTokenProviders();

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(Program));

        // Servicios 
        builder.Services.AddScoped<IStringService, StringService>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped(typeof(IApplication<>), typeof(Application<>));
        builder.Services.AddScoped(typeof(IDbContext<>), typeof(DbContext<>));
        builder.Services.AddScoped<ITokenHandlerService, TokenHandlerService>();
        builder.Services.AddScoped<IStockService, StockService>();

        // Manejo global de errores: respuestas { error } en vez de 500 con stack trace
        builder.Services.AddExceptionHandler<ManejadorDeErrores>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        // Tiene que ir primero para atrapar los errores de todo lo que sigue
        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ComponentesComputadoras.WebApi v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication(); 
        app.UseAuthorization();

        app.MapControllers();

        // Crear usuario Admin por defecto
        using (var scope = app.Services.CreateScope())
        {
            // 1) Crear la base de datos y aplicar las migraciones automaticamente.
            //    Asi quien baje el proyecto no necesita correr "Update-Database".
            var db = scope.ServiceProvider.GetRequiredService<DbDataAccess>();
            try
            {
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se pudo conectar a SQL Server.");
                Console.WriteLine("Cadena de conexion usada: " + builder.Configuration.GetConnectionString("DefaultConnection"));
                Console.WriteLine("Ningun servidor respondio. Revisa que el servicio 'SQL Server (...)' este iniciado (services.msc)");
                Console.WriteLine("o pone el nombre de tu servidor en appsettings.Local.json (ver LEEME.md).");
                Console.WriteLine("Detalle: " + ex.Message);
                Console.ResetColor();
                throw;
            }

            // 2) Cargar datos de ejemplo si la base esta vacia
            await DatosIniciales.CargarAsync(db);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var adminEmail = "admin@email.com";
            var adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingUser == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombres = "Admin",
                    Apellidos = "Principal",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Admin");
                }
            }
        }

        app.Run();
    }
}
