using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.Models
{
    public class Employee
    {
        [Key]
        [Display(Name = "Número de Empleado")]
        public int emp_no { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres")]

        [Display(Name = "Cédula")]
        public string ci { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime birth_date { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]