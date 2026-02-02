using Contracts;
using Entities.Models;
using FileNova.Exceptions;
using FileNova.Presentation.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.Text;


NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter () =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();

var builder = WebApplication.CreateBuilder(args);

// ==================================
// Configuración de NLog
// ==================================
var nlogConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");
LogManager.Setup().LoadConfigurationFromFile(nlogConfigFile, optional: false);

// ==================================
// Registro de servicios
// ==================================
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<PatchValidationFilterAttribute>();
builder.Services.ConfigueResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();


builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.ConfigureAuthorizationPolicies();



// AutoMapper
builder.Services.AddAutoMapper(typeof(FileNova.MappingProfile).Assembly);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});



// ==================================
// Configuración de controladores
// ==================================
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    //config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
    
    
})
.AddXmlDataContractSerializerFormatters() // Soporte XML
.AddCustomCSVFormatter()
.AddApplicationPart(typeof(FileNova.Presentation.AssemblyReference).Assembly)
.AddNewtonsoftJson();



// ==================================
// Construcción del pipeline
// ==================================
var app = builder.Build();

// Middleware de excepciones global
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Seguridad en producción
if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos")),
    RequestPath = "/archivos"
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseAuthentication();
app.UseAuthorization();

// ==================================
// Enrutamiento de controladores
// ==================================
app.MapControllers();




app.Run();
