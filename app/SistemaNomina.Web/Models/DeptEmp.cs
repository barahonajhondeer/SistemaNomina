using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class DeptEmp
    {
        [Key]
        [Column(Order = 1)]
        [Display(Name = "Número de Empleado")]
        public int emp_no { get; set; }
