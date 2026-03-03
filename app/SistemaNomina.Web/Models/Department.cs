using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.Models
{
    public class Department
    {
        [Key]
        [StringLength(10, ErrorMessage = "El código de departamento no puede exceder los 10 caracteres")]
        [Display(Name = "Código de Departamento")]
        public string dept_no { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre del Departamento")]
        public string dept_name { get; set; }

        // Relaciones
        public virtual ICollection<DeptEmp> DeptEmps { get; set; }
        public virtual ICollection<DeptManager> DeptManagers { get; set; }
    }
}
