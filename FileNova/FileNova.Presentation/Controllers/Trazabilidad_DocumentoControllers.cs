using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNova.Presentation.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    public class Trazabilidad_DocumentoControllers : ControllerBase
    {
        private readonly IServiceManager _service;
        public Trazabilidad_DocumentoControllers(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet("{Id_documento}/trazabilidades")]
        public async Task<IActionResult> GetTrazabilidadForDocumento(int Id_documento)
        {
            var trazabilidadDocumento = await _service.Trazabilidad_DocumentoService.GetAllTrazabilidad_DocumentoAsync(Id_documento, trackChanges: false);

            return Ok(trazabilidadDocumento);
        }

    }
}
