using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Documentos
{
    public class DocumentoForCreationDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public float Tamaño_KB { get; set; }
        public string? Ruta { get; set; }
        public DateTime Fecha_Subida { get; set; }
        public string? Id_Usuario { get; set; }
        public string? Tipo { get; set; }
    }
}
