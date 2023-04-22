namespace FinalProject.Models
{
    public class CategoryAssignment
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Assignment Assignment { get; set; }
        public int AssignmentId { get; set; }
    }
}
