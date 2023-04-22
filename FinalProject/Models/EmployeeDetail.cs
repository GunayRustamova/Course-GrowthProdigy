using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class EmployeeDetail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public bool Gender { get; set; }
        public int ExperienceYear { get; set; }
        public DateTime StartTime { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
    }
}
