using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Permiso
    {
        [Key]
        [Column("id_permiso")]
        public int Id_Permiso { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string? Nombre { get; set; }

        [MaxLength(255, ErrorMessage = "La descripción no puede tener más de 255 caracteres")]
        public string? Descripcion { get; set; }

        public List<Rol_Permiso> Rol_Permiso { get; set; } = [];
        

    }
}
