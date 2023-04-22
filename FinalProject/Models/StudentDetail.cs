using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class StudentDetail
    {
        public int Id { get; set; }
        public string FatherName { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool Gender { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
    }
}
