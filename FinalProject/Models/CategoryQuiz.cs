namespace FinalProject.Models
{
    public class CategoryQuiz
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Quiz Quiz { get; set; }
        public int QuizId { get; set; }
    }
}
