using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class DeptManager
    {
        [Key]
        [Column(Order = 1)]
        [Display(Name = "Número de Empleado (Gerente)")]
        public int emp_no { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        [Display(Name = "Código de Departamento")]
        public string dept_no { get; set; }

        [Key]
        [Column(Order = 3)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime from_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? to_date { get; set; }

        // Relaciones
        [ForeignKey("emp_no")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("dept_no")]
        public virtual Department Department { get; set; }
    }
}