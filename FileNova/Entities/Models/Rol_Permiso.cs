using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Rol_Permiso
    {
        
        // Clave compuesta
        [Required]
        public string? Id_Rol { get; set; }  // Id del IdentityRole
        public Role? Role { get; set; }

        [Required]
        public int Id_Permiso { get; set; }  // Id del Permiso
        public Permiso? Permiso { get; set; }
    }
}
