using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    public class SalaryViewModel
    {
        public int emp_no { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio")]
        [Display(Name = "Empleado")]
        public string EmpleadoNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El salario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser un valor positivo")]
        [DataType(DataType.Currency)]
        [Display(Name = "Salario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal salary { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime from_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? to_date { get; set; }

        // Para mostrar en listas
        public string? Estado => to_date == null ? "Activo" : "Inactivo";
    }
}