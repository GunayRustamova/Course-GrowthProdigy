using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class HistoryOfStudentsController : Controller
    {
        private readonly AppDbContext _db;
        public HistoryOfStudentsController(AppDbContext db)
        {
            _db = db;
        }

        #region History of Student İndex

        public async Task<IActionResult> Index()
        {
            List<Student> students = await _db.Students.
                                               Include(x => x.Group).
                                               Include(x => x.Category).
                                               Include(x => x.Course).
                                               Include(x => x.StudentDetail).
                                               ToListAsync();
            return View(students);
        }

        #endregion

    }
}
