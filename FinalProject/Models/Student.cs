namespace FinalProject.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsDeactive { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public StudentDetail StudentDetail { get; set; }
    }
}
