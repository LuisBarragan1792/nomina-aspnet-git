using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class DeptManager
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(10)]
        public string DeptNo { get; set; } = "";

        [Required]
        public int EmpNo { get; set; } // manager

        [Required]
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}