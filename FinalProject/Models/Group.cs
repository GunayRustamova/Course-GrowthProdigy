using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int GroupNumber { get; set; }
        public bool IsDeactive { get; set; }
        public int TotalPlaceCount { get; set; }
        public int EmptyPlaceCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<Student> Students { get; set; }
        public List<Lesson> Lessons { get; set; }
        public List<GroupAssignment> GroupAssignments { get; set; }
        public List<GroupQuiz> GroupQuizzes { get; set; }
    }
}