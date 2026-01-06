using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts.Interface
{
    public interface IDocumentoRepository
    {
        Task<PagedList<Documento>> GetAllDocumentos(DocumentoParameters documentoParameters, bool trackChanges);
        Task<Documento> GetDocumento(int Id_Documento, bool trackChanges);
        Task<IEnumerable<Documento>> GetDocumentoByIds(IEnumerable<int> Id_Documentos, bool trackChanges);
        
        void CreateDocumento(Documento documento);
        void DeleteDocumento(Documento documento);
    }
}
