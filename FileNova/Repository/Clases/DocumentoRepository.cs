using Contracts.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository.Clases
{
    public class DocumentoRepository : RepositoryBase<Documento>, IDocumentoRepository
    {
        public DocumentoRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<PagedList<Documento>> GetAllDocumentos(DocumentoParameters documentoParameters, bool trackChanges)
        {
            var documento = await FindByCondition(d => d.Estado == 1 && d.Fecha_Creacion >= documentoParameters.MinFecha && d.Fecha_Creacion <= documentoParameters.MaxFecha, trackChanges)
                .FilterDocumento(documentoParameters.MinFecha, documentoParameters.MaxFecha)
                .Search(documentoParameters.Busqueda)
                .Sort(documentoParameters.Orden)
                .ToListAsync();

            return PagedList<Documento>
                .ToPageList(documento, documentoParameters.PageNumber, documentoParameters.PageSize);
        }
            


        public async Task<Documento> GetDocumento(int Id_Documento, bool trackChanges)=>
            await FindByCondition(d => d.Id_Documento.Equals(Id_Documento) && d.Estado == 1, trackChanges)
            .SingleOrDefaultAsync();


        public async Task<IEnumerable<Documento>> GetDocumentoByIds(IEnumerable<int> Id_Documentos, bool trackChanges)=>
            await FindByCondition(d => Id_Documentos.Contains(d.Id_Documento) && d.Estado == 1, trackChanges)
            .ToListAsync();
        public void CreateDocumento(Documento documento)=>
            Create(documento);

        public void DeleteDocumento(Documento documento) =>
            Delete(documento);


        
    }
}
