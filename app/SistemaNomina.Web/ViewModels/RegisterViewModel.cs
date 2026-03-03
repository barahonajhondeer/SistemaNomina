using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El número de empleado es obligatorio")]
        [Display(Name = "Número de Empleado")]
        public int emp_no { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Confirmar la contraseña es obligatorio")]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmarClave { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Rol")]
        public string Rol { get; set; }
    }
}