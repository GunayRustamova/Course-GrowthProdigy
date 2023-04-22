namespace FinalProject.Models
{
    public class GroupAssignment
    {
        public int Id { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public Assignment Assignment { get; set; }
        public int AssignmentId { get; set; }
    }
}
