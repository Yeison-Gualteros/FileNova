using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Rol_Permiso
    {
        public int Id_Rol { get; set; }
        public int Id_Permiso { get; set; }

        public Permiso? Permiso { get; set; }
    }
}
