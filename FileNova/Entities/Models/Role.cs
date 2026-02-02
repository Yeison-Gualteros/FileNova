using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Role : IdentityRole
    {
        public ICollection<Rol_Permiso> Rol_Permisos { get; set; }
        public ICollection<Permiso> Permisos { get; set; }
        //public ICollection<User_Role> User_Roles { get; set; } = new List<User_Role>();
    }

}
