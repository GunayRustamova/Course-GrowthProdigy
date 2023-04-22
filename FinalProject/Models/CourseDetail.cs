using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class CourseDetail
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string CourseDuration { get; set; }
        public Course Course { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
    }
}
