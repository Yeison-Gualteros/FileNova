using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public override string Email { get; set; }

        public string? RefreshTokken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        //[ForeignKey("Rol")]
        //[Column("id_rol")]
        //public int Id_Rol { get; set; }


        public ICollection<Documento>? Documentos { get; set; }
        public ICollection<Trazabilidad_Documento>? Trazabilidad_Documentos { get; set; }
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
