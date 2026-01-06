using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interface
{
    public interface ITrazabilidad_DocumentoRepository
    {
        Trazabilidad_Documento GetAllTrazabilidad_Documentos(int Id_documento, int Id_Trazabilidad, bool trackChanges);
        void CreateTrazabilidadDocumento(Trazabilidad_Documento trazabilidad_Documento);
        Task<IEnumerable<Trazabilidad_Documento>> GetAllByDocumentoAsync(int idDocumento, bool trackChanges);

        //IEnumerable<Trazabilidad_Documento> GetAll(bool trackChanges);
        //IEnumerable<Trazabilidad_Documento> GetByDocumentoId(int Id_Documento, bool trackChanges);
    }
}
