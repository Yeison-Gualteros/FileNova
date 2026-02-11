using Entities.Models;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.Documentos;
using Shared.RequestFeatures;
using System.Collections.Generic;

namespace Service.Contracts
{
    public interface IDocumentoService
    {
        //Obtener todos los documentos
        Task<(IEnumerable<DocumentoDTO> documentos, MetaData metaData)> GetAllDocumentos(DocumentoParameters documentoParameters, bool trackChanges);

        Task<DocumentoDTO> GetDocumento(int Id_Documento, bool trackChanges);

        //Obtener un documento por ID
        Task<IEnumerable<DocumentoDTO>> GetDocumentoByIds(IEnumerable<int> Id_Documento, bool trackChanges);


        //Crear un documento
        Task<DocumentoDTO> CreateDocumentoAsync(DocumentoForCreationDto documentoForCreation, IFormFile archivo, bool trackChanges);


        //Eliminar un documento
        Task DeleteDocumento(int Id_Documento, bool trackChanges);

        //Guardar Documento
        Task<string> GuardarArchivoAsync(IFormFile archivo);

        //Actualizar un documento
        Task<DocumentoDTO> ActualizarDocumentoAsync(int Id_Documento, DocumentoForUpdateDto documentoForUpdate, IFormFile archivo, bool trackChanges);

        //Task<(DocumentoForUpdateDto documentoToPacht, Documento documentoEntity)> GetDocumentoForPatch(int Id_Documento, bool trackChanges);

        //Task SaveChangesForPatch(DocumentoForUpdateDto documentoToPatch, Documento documentoEntity);
    }
}
