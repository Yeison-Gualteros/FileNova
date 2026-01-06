
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;


namespace Entities.Models
{
    public class Documento
    {
        [Key]
        [Column("id_documento")]
        public int Id_Documento { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
        public string? Nombre { get; set; }
        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string? Descripcion { get; set; }
        public float Tamaño_KB { get; set; }
        public DateTime Fecha_Subida { get; set; }

        [Column("ruta")]
        public string Ruta { get; set; } = string.Empty;

        [Column("fecha_creacion")]
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

        [Column("fecha_modificacion")]
        public DateTime Fecha_Modificacion { get; set; } = DateTime.Now;

        public string? Tipo { get; set; }
        
        public int Estado { get; set; } = 1; 

        // Usuario
        [ForeignKey("Usuario")]
        [Column("id_usuario")]
        public string? Id_Usuario { get; set; }
        public User? User { get; set; }

        // Trazabilidad
        public ICollection<Trazabilidad_Documento>? Trazabilidad_Documentos { get; set; }

      

    }
}
