using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Trazabilidad_Documento
    {
        [Key]
        [Column("id_trazabilidad_documento")]
        public int Id_Trazabilidad { get; set; }
        public string? Ruta_Antigua { get; set; }
        public string? Ruta_Nueva { get; set; }
        public string? Accion { get; set; }
        public DateTime Fecha_Cambio { get; set; }

        // Documento
        [ForeignKey("Documento")]
        [Column("id_documento")]
        public int Id_Documento { get; set; }
        public Documento? Documento { get; set; }

        // Usuario
        [ForeignKey("User")]
        [Column("id_usuario")]
        public string? Id_Usuario { get; set; }
        public User? User { get; set; }
    }
}
