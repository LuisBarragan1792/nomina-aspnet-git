using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        public int? EmpNo { get; set; } // opcional

        [Required, StringLength(40)]
        public string Usuario { get; set; } = "";

        [Required]
        public string ClaveHash { get; set; } = "";

        [Required, StringLength(20)]
        public string Rol { get; set; } = "RRHH"; // Admin o RRHH

        public bool IsActive { get; set; } = true;
    }
}