using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class Trazabilidad_DocumentoDTO
    {
        public int Id_Trazabilidad { get; set; }          // Id del registro de trazabilidad
        public string? Ruta_Antigua { get; set; }          // Ruta del archivo antes del cambio
        public string? Ruta_Nueva { get; set; }           // Ruta del archivo después del cambio
        public string? Accion { get; set; }              // Ej: "Actualización", "Renombrado"
        public DateTime Fecha_Cambio { get; set; }       // Fecha y hora del cambio
        public int Id_Documento { get; set; }            // Id del documento al que pertenece
        public int Id_Usuario { get; set; }
    }
}
