using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class Department
    {
        [Key, StringLength(10)]
        public string DeptNo { get; set; } = "";

        [Required, StringLength(80)]
        public string DeptName { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}