using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string File { get; set; }
        [NotMapped]
        public IFormFile AddFile { get; set; }
        public List<GroupAssignment> GroupAssignments { get; set; }
        public List<CourseAssignment> CourseAssignments { get; set; }
        public List<CategoryAssignment> CategoryAssignments { get; set; }
        //public List<Question> Questions { get; set; }
    }
}
