using System.ComponentModel.DataAnnotations;

namespace Evaluación.Models
{
    public class Department
    {
        [Key]
        public string DeptNo { get; set; }

        [Required]
        public string DeptName { get; set; }

        public bool Activo { get; set; } = true;
    }
}