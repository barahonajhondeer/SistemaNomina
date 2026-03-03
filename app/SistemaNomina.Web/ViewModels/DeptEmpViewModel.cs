using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    public class DeptEmpViewModel
    {
        [Required(ErrorMessage = "El empleado es obligatorio")]
        [Display(Name = "Empleado")]
        public int emp_no { get; set; }

        public string? EmpleadoNombre { get; set; }  // Nullable porque no viene del formulario

        [Required(ErrorMessage = "El departamento es obligatorio")]
        [Display(Name = "Departamento")]
        public string dept_no { get; set; } = string.Empty;

        public string? DepartamentoNombre { get; set; }  // Nullable porque no viene del formulario

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime from_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        public DateTime? to_date { get; set; }

        // Validación personalizada
        public static ValidationResult ValidarFechas(DateTime? to_date, ValidationContext context)
        {
            var instance = (DeptEmpViewModel)context.ObjectInstance;

            if (to_date.HasValue && to_date.Value < instance.from_date)
            {
                return new ValidationResult("La fecha de fin no puede ser menor a la fecha de inicio.");
            }

            return ValidationResult.Success;
        }
    }
}