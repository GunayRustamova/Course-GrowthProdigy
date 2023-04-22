using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FinalProject.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }
        public string Image { get; set; }
        public bool IsDeactive { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public Position Position { get; set; }
        public int PositionId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public EmployeeDetail EmployeeDetail { get; set; }

        //public PaidSalary PaidSalary { get; set; }

    }
}
