using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Documentos
{
    public class DocumentoForUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string?   Id_Usuario { get; set; }
        public int Estado { get; set; }
        public string? Tipo { get; set; }
    }
}
