using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    public class TitleViewModel
    {
        public int emp_no { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio")]
        [Display(Name = "Empleado")]
        public string EmpleadoNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres")]
        [Display(Name = "Título/Cargo")]
        public string title { get; set; } = string.Empty;

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