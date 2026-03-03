using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class Salary
    {
        [Key]
        public int emp_no { get; set; }

        [Key]
        public DateTime from_date { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser positivo")]
        [DataType(DataType.Currency)]
        public decimal salary { get; set; }

        [DataType(DataType.Date)]
        public DateTime? to_date { get; set; }

        // Relaciones
        [ForeignKey("emp_no")]
        public virtual Employee Employee { get; set; }
    }
}
