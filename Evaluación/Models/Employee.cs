using System.ComponentModel.DataAnnotations;

namespace Evaluación.Models
{
    public class Employee
    {
        [Key]
        public int EmpNo { get; set; }

        [Required]
        public string Ci { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Correo { get; set; }

        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }

        public bool Activo { get; set; } = true;
    }
}