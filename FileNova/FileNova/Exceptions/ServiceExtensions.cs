using Contracts;
using Entities.Models;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace FileNova.Exceptions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("X-Pagination");
                });
            });

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options =>
            {
                // Configuraciones específicas de IIS pueden ir aquí
            });

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(typeof(FileNova.MappingProfile));

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.hateoas+json");
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.apiroot+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                  .OfType<XmlDataContractSerializerOutputFormatter>()?
                  .FirstOrDefault();

                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.hateoas+xml");
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.codemaze.apiroot+xml");
                }

            });
        }

        public static void ConfigueResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders((expirationOpt) =>
            {
                expirationOpt.MaxAge = 65;
                expirationOpt.CacheLocation = CacheLocation.Private;
            },
            (validationOpt) =>
            {
                validationOpt.MustRevalidate = true;
            });

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, Role>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind("JwtSettings", jwtConfiguration); // Se asegura de que los valores estén cargados desde appsettings.json
            var key = Encoding.UTF8.GetBytes(jwtConfiguration.Key);  // La clave debe coincidir con la de appsettings.json

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtConfiguration.ValidIssuer,  // Debe coincidir con el valor en appsettings.json
                    ValidAudience = jwtConfiguration.ValidAudience,  // Debe coincidir con el valor en appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(key)  // Asegúrate de que la clave sea la misma
                };
            });
        }


        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) => 
            services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));

        public static void ConfigureAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DOCUMENTOS_VER", policy =>
                    policy.RequireClaim("permission", "DOCUMENTOS_VER"));

                options.AddPolicy("DOCUMENTOS_CREAR", policy =>
                    policy.RequireClaim("permission", "DOCUMENTOS_CREAR"));

                options.AddPolicy("DOCUMENTOS_EDITAR", policy =>
                    policy.RequireClaim("permission", "DOCUMENTOS_EDITAR"));

                options.AddPolicy("DOCUMENTOS_ELIMINAR", policy =>
                    policy.RequireClaim("permission", "DOCUMENTOS_ELIMINAR"));

                options.AddPolicy("ROLES_ADMIN", policy =>
                    policy.RequireClaim("permission", "ROLES_ADMIN"));
            });
        }

    }
}
