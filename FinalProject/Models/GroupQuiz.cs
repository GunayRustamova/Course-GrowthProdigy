namespace FinalProject.Models
{
    public class GroupQuiz
    {
        public int Id { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public Quiz Quiz { get; set; }
        public int QuizId { get; set; }
    }
}
