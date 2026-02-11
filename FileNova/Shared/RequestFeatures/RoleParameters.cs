using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class RoleParameters : RequestParameters
    {
        public RoleParameters()
        {
            Orden = "Name";        // columna por defecto
            Direccion = "asc";     // dirección por defecto
        }

        public string? Busqueda { get; set; }
        public string Orden { get; set; }       // columna
        public string Direccion { get; set; }   // "asc" o "desc"
    }
}
