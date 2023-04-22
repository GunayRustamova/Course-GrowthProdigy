using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public CourseDetail CourseDetail { get; set; }
        public List<Category> Categories { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Group> Groups { get; set; }
        public List<Student> Students { get; set; }
        public List<Lesson> Lessons { get; set; }
        public List<CourseAssignment> CourseAssignments { get; set; }
        public List<CourseQuiz> CourseQuizzes { get; set; }

    }
}
