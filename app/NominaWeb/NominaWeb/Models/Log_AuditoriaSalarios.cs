using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaWeb.Models
{
    public class Log_AuditoriaSalarios
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Usuario { get; set; } = "";

        [Required]
        public DateTime FechaActualizacion { get; set; }

        [Required, StringLength(250)]
        public string DetalleCambio { get; set; } = "";

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salario { get; set; }

        [Required]
        public int EmpNo { get; set; }
    }
}