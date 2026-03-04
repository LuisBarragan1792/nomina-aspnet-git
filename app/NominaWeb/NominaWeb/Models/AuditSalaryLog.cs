using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class AuditSalaryLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Usuario { get; set; }

        public DateTime FechaActualizacion { get; set; }

        [Required]
        public string DetalleCambio { get; set; }

        public decimal Salario { get; set; }

        public int EmpNo { get; set; }
    }
}