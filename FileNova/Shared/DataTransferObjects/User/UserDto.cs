using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.User
{
    public class UserDto
    {
        public string? Id { get; set; }  // cambiar a Id si tu front-end espera id
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Nombre { get; set; }  // corregido
        public string? Apellido { get; set; }
        public int Estado { get; set; }
        public List<int> Permisos { get; set; } = new List<int>();
        public string Rol { get; set; }
    }

}
