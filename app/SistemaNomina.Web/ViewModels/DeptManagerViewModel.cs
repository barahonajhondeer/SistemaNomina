using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    public class DeptManagerViewModel
    {
        [Required(ErrorMessage = "El departamento es obligatorio")]
        [Display(Name = "Departamento")]
        public string dept_no { get; set; } = string.Empty;

        [Display(Name = "Nombre del Departamento")]
        public string? DepartamentoNombre { get; set; }

        [Required(ErrorMessage = "El gerente es obligatorio")]
        [Display(Name = "Gerente")]
        public int emp_no { get; set; }

        [Display(Name = "Nombre del Gerente")]
        public string? GerenteNombre { get; set; }

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