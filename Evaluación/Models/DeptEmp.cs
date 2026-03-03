using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evaluación.Models
{
    public class DeptEmp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmpNo { get; set; }

        [Required]
        public string DeptNo { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [ValidateNever]   
        public Employee Employee { get; set; }

        [ValidateNever]   
        public Department Department { get; set; }
    }
}