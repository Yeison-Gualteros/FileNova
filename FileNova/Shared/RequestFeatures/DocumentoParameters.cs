using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class DocumentoParameters : RequestParameters
    {
        public DocumentoParameters() => Orden = "nombre";

        public DateTime MinFecha { get; set; }
        public DateTime MaxFecha { get; set; } = DateTime.MaxValue;

        public bool ValidFechaRango => MaxFecha > MinFecha;

        public string? Busqueda { get; set; }

    }
}
