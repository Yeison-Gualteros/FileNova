using Contracts.Interface;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IDocumentoRepository Documento { get; }
        ISolicitudRepository Solicitud { get; }
        ITrazabilidad_DocumentoRepository trazabilidad_Documento { get; }
        IRoleRepository Role { get; }

        Task SaveAsync();
    }
}
