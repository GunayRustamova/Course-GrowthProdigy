using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser> 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
               
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDetail> CourseDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentDetail> StudentDetails { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<GroupAssignment> GroupAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<CategoryAssignment> CategoryAssignments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<GroupQuiz> GroupQuizzes { get; set; }
        public DbSet<CourseQuiz> CourseQuizzes { get; set; }
        public DbSet<CategoryQuiz> CategoryQuizzes { get; set; }
        public DbSet<Expenditure> Expenditures { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Cash> Cashes { get; set; }
        public DbSet<PaidSalary> PaidSalaries { get; set; }
        public DbSet<MailMessage> MailMessages { get; set; }

    }
}
