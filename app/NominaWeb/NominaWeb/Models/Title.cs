using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class Title
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmpNo { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = "";

        [Required]
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}