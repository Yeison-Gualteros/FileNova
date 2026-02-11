using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Documentos
{
    public record class DocumentoDTO
    {
        public int Id_Documento { get; init; }
        public string? Nombre { get; init; }
        public string? Descripcion { get; init; }
        public float Tamaño_KB { get; init; }
        public string? Ruta { get; init; }
        public DateTime Fecha_Creacion { get; init; }
        public int Id_Usuario { get; init; }
    }
}
