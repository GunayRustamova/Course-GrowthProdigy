namespace FinalProject.Models
{
    public class CourseQuiz
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Quiz Quiz { get; set; }
        public int QuizId { get; set; }
    }
}
