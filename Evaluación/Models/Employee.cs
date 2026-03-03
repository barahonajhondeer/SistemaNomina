using Evaluación.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.Models
{
    public class Employee
    {
        [Key]
        public int emp_no { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20)]
        public string ci { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime birth_date { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Primer Nombre")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Primer Apellido")]
        public string last_name { get; set; }

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(1)]
        public string gender { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Contratación")]
        public DateTime hire_date { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100)]
        public string correo { get; set; }

        // Relaciones
        public virtual ICollection<DeptEmp> DeptEmps { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
        public virtual ICollection<Title> Titles { get; set; }
        public virtual ICollection<DeptManager> DeptManagers { get; set; }
        public virtual User User { get; set; }
    }
}