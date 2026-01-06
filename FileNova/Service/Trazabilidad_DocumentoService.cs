using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Trazabilidad_DocumentoService : ITrazabilidad_DocumentoService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public Trazabilidad_DocumentoService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task CreateTrazabilidadDocumento(int Id_Documento, string accion /*string usuario*/)
        {
            var trazabilidad = new Trazabilidad_Documento
            {
                Id_Documento = Id_Documento,
                Accion = accion,
                //Usuario = usuario,
                Fecha_Cambio = DateTime.Now

            };
            _repository.trazabilidad_Documento.CreateTrazabilidadDocumento(trazabilidad);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<Trazabilidad_DocumentoDTO>> GetAllTrazabilidad_DocumentoAsync(int idDocumento, bool trackChanges)
        {
            var documento = await _repository.Documento.GetDocumento(idDocumento, trackChanges);

            if (documento is null)
                throw new DocumentoNotFoundException(idDocumento);

            var trazabilidadesDb = await _repository.trazabilidad_Documento.GetAllByDocumentoAsync(idDocumento, trackChanges);

            return _mapper.Map<IEnumerable<Trazabilidad_DocumentoDTO>>(trazabilidadesDb);
        }
    }
}
