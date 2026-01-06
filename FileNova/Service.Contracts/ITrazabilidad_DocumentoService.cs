using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ITrazabilidad_DocumentoService
    {
        Task CreateTrazabilidadDocumento(int Id_Documento, string accion /*string usuario*/);
        Task<IEnumerable<Trazabilidad_DocumentoDTO>> GetAllTrazabilidad_DocumentoAsync(int idDocumento, bool trackChanges);


    }
}
