using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class PaidSalary
    {
        public int Id { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public Employee Employee { get; set; }

        public int EmployeeId { get; set; }
        public float Money { get; set; }
        public AppUser AppUser { get; set; }

        public string AppUserId { get; set; }
    }
}
