using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Shared.DataTransferObjects.Documentos;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        

        public DocumentoService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            
        }

        //Obtener todos los documentos
        public async Task<(IEnumerable<DocumentoDTO> documentos, MetaData metaData)> GetAllDocumentos(DocumentoParameters documentoParameters, bool trackChanges)
        {
            if (!documentoParameters.ValidFechaRango)
                throw new MaxFechaRangoBadRequestException();

            var documentosWithMetaData = await _repository.Documento.GetAllDocumentos(documentoParameters, trackChanges);
            var documentosDto = _mapper.Map<IEnumerable<DocumentoDTO>>(documentosWithMetaData);
           

            return (documentos: documentosDto, metaData: documentosWithMetaData.MetaData);
        }

        public async Task<DocumentoDTO> GetDocumento(int Id_Documento, bool trackChanges)
        {
            var documentoFromDb = await _repository.Documento.GetDocumento(Id_Documento, trackChanges);
            if (documentoFromDb is null)
                throw new DocumentoNotFoundException(Id_Documento);
            return _mapper.Map<DocumentoDTO>(documentoFromDb);
        }

        //Obtener documento por ID
        public async Task<IEnumerable<DocumentoDTO>> GetDocumentoByIds(IEnumerable<int> idDocumento, bool trackChanges)
        {
            
            if (idDocumento is null)
                throw new IdParametersBadRequestException();

            var documentoEntities = await _repository.Documento.GetDocumentoByIds(idDocumento, trackChanges);
            if (idDocumento.Count() != documentoEntities.Count())
                throw new CollectionByIdsBadRequestException();

            var documentosToReturn = _mapper.Map<IEnumerable<DocumentoDTO>>(documentoEntities);

            return documentosToReturn;
        }

        public async Task<string> GuardarArchivoAsync(IFormFile archivo)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return "/archivos/" + fileName;
        }

        //Crear un documento
        public async Task<DocumentoDTO> CreateDocumentoAsync(DocumentoForCreationDto documentoForCreation, IFormFile archivo, bool trackChanges)
        {
            var documentoEntity = _mapper.Map<Documento>(documentoForCreation);
            documentoEntity.Estado = 1;
            documentoEntity.Ruta = documentoForCreation.Ruta; // ya asignado en el controlador
            documentoEntity.Tamaño_KB = documentoForCreation.Tamaño_KB;
            documentoEntity.Fecha_Subida = documentoForCreation.Fecha_Subida;
            documentoEntity.Tipo = "PDF";

            _repository.Documento.CreateDocumento(documentoEntity);
            await _repository.SaveAsync();

            var trazabilidad = new Trazabilidad_Documento
            {
                Id_Documento = documentoEntity.Id_Documento,
                Ruta_Antigua = documentoEntity.Ruta,
                Ruta_Nueva = documentoEntity.Ruta,
                Accion = "Creacion",
                Fecha_Cambio = DateTime.Now,
                Id_Usuario = documentoForCreation.Id_Usuario
            };

            _repository.trazabilidad_Documento.CreateTrazabilidadDocumento(trazabilidad);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error EF al guardar: {ex.InnerException?.Message ?? ex.Message}");
                throw; // así Postman mostrará 500, pero en logs sabrás qué está mal
            }

            return _mapper.Map<DocumentoDTO>(documentoEntity);
        }

        //Actualizar Documento
        public async Task<DocumentoDTO> ActualizarDocumentoAsync(int Id_Documento, DocumentoForUpdateDto documentoForUpdate, IFormFile? archivo, bool trackChanges)
        {
            var documentoEntity = await _repository.Documento.GetDocumento(Id_Documento, trackChanges);

            if (documentoEntity is null)
                throw new DocumentoNotFoundException(Id_Documento);

            string rutaAntigua = documentoEntity.Ruta;
            string nombreAntiguo = documentoEntity.Nombre;
            string descripcionAntigua = documentoEntity.Descripcion;
            string tipoAntiguo = documentoEntity.Tipo;


            //actualiza valores basicos
            _mapper.Map(documentoForUpdate, documentoEntity);

            
            documentoEntity.Tipo = "PDF";
            documentoEntity.Estado = 1;
            
            string rutaNueva = documentoEntity.Ruta;
            documentoEntity.Fecha_Modificacion = DateTime.Now;

            //Si el usuario subió un archivo nuevo
            if (archivo != null)
            {
                rutaNueva = await GuardarArchivoAsync(archivo);
                documentoEntity.Ruta = rutaNueva;
                documentoEntity.Tamaño_KB = archivo.Length / 1024f;
                
                documentoEntity.Estado = 1;
            }

            bool huboCambios = false;

            if (rutaNueva != rutaAntigua)
            {
                huboCambios = true;
            }

            if (nombreAntiguo != documentoEntity.Nombre || tipoAntiguo != documentoEntity.Tipo || descripcionAntigua != documentoEntity.Descripcion)
            {
                huboCambios = true;
            }

            if (huboCambios)
            {
                var trazabilidad = new Trazabilidad_Documento
                {
                    Id_Documento = documentoEntity.Id_Documento,
                    Ruta_Antigua = rutaAntigua,
                    Ruta_Nueva = rutaNueva,
                    Accion = "Actualizacion",
                    Fecha_Cambio = DateTime.Now,
                    Id_Usuario = documentoForUpdate.Id_Usuario,

                   
                };
                _repository.trazabilidad_Documento.CreateTrazabilidadDocumento(trazabilidad);
            }

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error EF al guardar: {ex.InnerException?.Message ?? ex.Message}");
                throw; 
            }

            return _mapper.Map<DocumentoDTO>(documentoEntity);

        }

        //Eliminar un documento
        public async Task DeleteDocumento(int Id_Documentos, bool trackChanges)
        {
            var documentoEntity = await _repository.Documento.GetDocumento(Id_Documentos, trackChanges: true);

            if (documentoEntity is null)
                throw new DocumentoNotFoundException(Id_Documentos);

            documentoEntity.Estado = 0;
            await _repository.SaveAsync();
            
            var trazabilidad = new Trazabilidad_Documento
            {
                Id_Documento = documentoEntity.Id_Documento,
                Ruta_Antigua = documentoEntity.Ruta,
                Ruta_Nueva = documentoEntity.Ruta,
                Accion = "Eliminacion",
                Fecha_Cambio = DateTime.Now,
                Id_Usuario = "1" 
            };

            _repository.trazabilidad_Documento.CreateTrazabilidadDocumento(trazabilidad);
            
            //_mapper.Map<DocumentoDTO>(documentoEntity);

            await _repository.SaveAsync();
        }


    }
}
