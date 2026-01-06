using Contracts.Interface;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IDocumentoRepository Documento { get; }
        ISolicitudRepository Solicitud { get; }
        ITrazabilidad_DocumentoRepository trazabilidad_Documento { get; }

        Task SaveAsync();
    }
}
