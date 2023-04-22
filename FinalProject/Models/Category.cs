using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Group> Groups { get; set; }
        public List<Student> Students { get; set; }
        public List<Lesson> Lessons { get; set; }
        public List<CategoryAssignment> CategoryAssignments { get; set; }
        public List<CategoryQuiz> CategoryQuizzes { get; set; }

    }
}
