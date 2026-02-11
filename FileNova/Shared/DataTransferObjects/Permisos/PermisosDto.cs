using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Permisos
{
    public record class PermisosDto
    {
        public int Id_Permiso { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public bool Heredado { get; set; }   // viene del rol
        public bool Selected { get; set; }   // checkbox marcado
        public bool Disabled { get; set; }
        public string Source { get; set; } // "role" | "user"

    }
}
