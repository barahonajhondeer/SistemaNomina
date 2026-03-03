using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.Models
{
    public class LogAuditoriaSalario
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string usuario { get; set; }

        [Required]
        public DateTime fechaActualizacion { get; set; }

        [Required]
        [StringLength(500)]
        public string DetalleCambio { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal salario { get; set; }

        [Required]
        public int emp_no { get; set; }
    }
}