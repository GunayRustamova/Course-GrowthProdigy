namespace FinalProject.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDeactive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        //public Room Room { get; set; }
        //public int RoomId { get; set;}
    }
}
