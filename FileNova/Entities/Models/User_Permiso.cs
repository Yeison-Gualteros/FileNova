using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User_Permiso
    {
        
        public string? UserId { get; set; }
        public int Id_Permiso { get; set; }

        // Relación de navegación si estás usando EF
        public virtual User? User { get; set; }
        public virtual Permiso? Permiso { get; set; }
    }
}
