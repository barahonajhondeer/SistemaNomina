using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaNomina.Web.Models
{
    public class Title
    {
        [Key]
        public int emp_no { get; set; }

        [Key]
        public string title { get; set; }

        [Key]
        public DateTime from_date { get; set; }

        [DataType(DataType.Date)]
        public DateTime? to_date { get; set; }

        // Relaciones
        [ForeignKey("emp_no")]
        public virtual Employee Employee { get; set; }
    }
}