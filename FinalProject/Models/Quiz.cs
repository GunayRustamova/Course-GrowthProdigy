using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public string File { get; set; }
        [NotMapped]
        public IFormFile AddFile { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<GroupQuiz> GroupQuizzes { get; set; }
        public List<CourseQuiz> CourseQuizzes { get; set; }
        public List<CategoryQuiz> CategoryQuizzes { get; set; }
        //public List<Question> Questions { get; set; }
    }
}
