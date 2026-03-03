using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class User
    {
        [Key]
        public int emp_no { get; set; }

        [Required]
        public string usuario { get; set; } = string.Empty;  // Inicializar

        [Required]
        public string clave { get; set; } = string.Empty;    // Inicializar

        [Required]
        public string rol { get; set; } = string.Empty;      // Inicializar

        // Relación - puede ser null
        public virtual Employee? Employee { get; set; }
    }
}