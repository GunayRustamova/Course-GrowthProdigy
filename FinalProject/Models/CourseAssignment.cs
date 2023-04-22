namespace FinalProject.Models
{
    public class CourseAssignment
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Assignment Assignment { get; set; }
        public int AssignmentId { get; set; }

    }
}
