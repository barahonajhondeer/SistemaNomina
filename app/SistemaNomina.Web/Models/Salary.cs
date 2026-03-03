using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class Salary
    {
        [Key]
        [Column(Order = 1)]
        [Display(Name = "Número de Empleado")]
        public int emp_no { get; set; }

        [Key]
        [Column(Order = 2)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime from_date { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser un valor positivo")]
        [DataType(DataType.Currency)]
        [Display(Name = "Salario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal salary { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? to_date { get; set; }

        // Relaciones
        [ForeignKey("emp_no")]
        public virtual Employee Employee { get; set; }
    }
}