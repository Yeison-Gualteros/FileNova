using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.Documentos;
using Shared.RequestFeatures;
using System.Text.Json;


namespace FileNova.Presentation.Controllers
{   
    [Route("api/documentos")]
    [ApiController]
    public class DocumentoControllers : ControllerBase
    {
        private readonly IServiceManager _service;

        public DocumentoControllers(IServiceManager service)
        {
            _service = service;
        }

        //Get /api/documetnos
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllDocumentos([FromQuery] DocumentoParameters documentoParameters)
        {
            var documentos = await _service.DocumentoService
                .GetAllDocumentos(documentoParameters, trackChanges: false);

            Response.Headers.Add(
                "X-Pagination",
                JsonSerializer.Serialize(documentos.metaData)
            );

            return Ok(documentos.documentos);
        }


        // Get /api/documentos/{Id_Documento}
        [HttpGet("{Id_Documento:int}", Name = "GetDocumento")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Administrador")]
        //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        //[HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetDocumentoById(int Id_Documento)
        {
            var documento = await _service.DocumentoService.GetDocumento(Id_Documento, trackChanges: false);
            
            return Ok(documento);
        }

        // Post /api/documento/
        [HttpPost("upload", Name = "CreateDocumento")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> SubirDocumento([FromForm] DocumentoForCreationDto documento, [FromForm] IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se envió ningún archivo.");

            // Validar extensiones permitidas
            var extensionesPermitidas = new[] { ".pdf", ".docx" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension))
                return BadRequest("Tipo de archivo no permitido.");

            // Validar tamaño máximo (5 MB)
            if (archivo.Length > 5 * 1024 * 1024)
                return BadRequest("Archivo demasiado grande, máximo 5MB.");

            // Guardar archivo en wwwroot/archivos
            var nombreSeguro = Path.GetRandomFileName() + extension;
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var rutaArchivo = Path.Combine(carpeta, nombreSeguro);
            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // **Asignar ruta y otros campos obligatorios ANTES de llamar al servicio**
            documento.Ruta = "/archivos/" + nombreSeguro;
            documento.Tamaño_KB = archivo.Length / 1024f;
            documento.Fecha_Subida = DateTime.Now;

            // Llamar al servicio
            var documentoEntity = await _service.DocumentoService.CreateDocumentoAsync(documento, archivo, trackChanges: false);

            return Ok(documentoEntity);
        }

        [HttpPut("{Id_Documento:int}", Name = "ActualizarDocumento")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarDocumento(int Id_Documento, [FromForm] DocumentoForUpdateDto documento, [FromForm] IFormFile? archivo)
        {
            if (documento is null)
                return BadRequest("No se envió información del documento");

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var documentoActualizado = await _service.DocumentoService.ActualizarDocumentoAsync(
                Id_Documento,
                documento,
                archivo,
                trackChanges: true
            );

            return Ok(documentoActualizado);
        }

        // delete /api/documentos/{Id_Documento}
        [HttpDelete("{Id_Documento:int}", Name = "EliminarDocumento")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteDocumento(int Id_Documento)
        {
            await _service.DocumentoService.DeleteDocumento(Id_Documento, trackChanges: false);

            
            return NoContent();
        }

        [HttpOptions(Name = "GetDocumetnosOptions")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Administrador")]
        public IActionResult GetDocumetnosOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT");
            return Ok();
        }

        
    }
}
