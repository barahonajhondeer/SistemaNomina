using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.Models
{
    public class LogAuditoriaSalario
    {
        [Key]
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Usuario")]
        public string usuario { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Actualización")]
        public DateTime fechaActualizacion { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Detalle del Cambio")]
        public string DetalleCambio { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Salario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal salario { get; set; }

        [Required]
        [Display(Name = "Número de Empleado")]
        public int emp_no { get; set; }
    }
}