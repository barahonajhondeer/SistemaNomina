using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class DeptManager
    {
        [Key]
        public int emp_no { get; set; }

        [Key]
        public string dept_no { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime from_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        public DateTime? to_date { get; set; }

        // Relaciones
        [ForeignKey("emp_no")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("dept_no")]
        public virtual Department Department { get; set; }
    }
}