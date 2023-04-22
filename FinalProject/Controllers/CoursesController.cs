using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class CoursesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CoursesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Course's Index

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Courses.Count() / 3);
            List<Course> courses = await _db.Courses.Skip((page - 1) * 3).Take(3).Include(x => x.CourseDetail).Include(x => x.Categories).ToListAsync();
            return View(courses);
        }

        #endregion

        #region Course's Create

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Course course)
        {
            bool isExist = await _db.Courses.Include(x => x.Categories).AnyAsync(x => x.Name == course.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Course is already exist");
                return View();
            }
            #region Course Create Photo

            if (course.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please, choose a photo");
                return View();
            }
            if (!course.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please, choose an image file");
                return View();
            }
            if (course.Photo.IsOlderTwoMb())
            {
                ModelState.AddModelError("Photo", "Image max 2mb");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "images");
            course.Image = await course.Photo.SaveFileAsync(folder);

            #endregion

            await _db.Courses.AddAsync(course);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Course's Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.Include(x => x.Employees).Include(x => x.Lessons).Include(x => x.CourseAssignments).Include(x => x.Categories).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }
            return View(dbCourse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, Course course)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.Include(x => x.Employees).Include(x => x.Lessons).Include(x => x.CourseAssignments).Include(x => x.Categories).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }

            bool isExist = await _db.Courses.AnyAsync(x => x.Name == course.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Course is already exist");
                return View(dbCourse);
            }

            #region Update Photo

            if (course.Photo != null)
            {
                if (!course.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Zəhmət olmasa bir şəkil fayli seçin");
                    return View(dbCourse);
                }
                if (course.Photo.IsOlderTwoMb())
                {
                    ModelState.AddModelError("Photo", "Şəkil max. 2 mb ola bilər");
                    return View(dbCourse);
                }

                string folder = Path.Combine(_env.WebRootPath, "images");
                course.Image = await course.Photo.SaveFileAsync(folder);
                dbCourse.Image = course.Image;
            }

            #endregion

            dbCourse.Name = course.Name;
            dbCourse.CourseDetail.Description = course.CourseDetail.Description;
            dbCourse.CourseDetail.Amount = course.CourseDetail.Amount;
            dbCourse.CourseDetail.CourseDuration = course.CourseDetail.CourseDuration;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region  Courses' Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.Include(x => x.Categories).Include(x => x.CourseDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }
            return View(dbCourse);
        }

        #endregion

        #region Course's Activity

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }
            if (dbCourse.IsDeactive)
            {
                dbCourse.IsDeactive = false;
            }
            else
            {
                dbCourse.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Course's Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.Include(x => x.Categories).Include(x => x.Employees).Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }
            return View(dbCourse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Course dbCourse = await _db.Courses.Include(x => x.Categories).Include(x => x.Employees).Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            if (dbCourse == null)
            {
                return BadRequest();
            }
            _db.Courses.Remove(dbCourse);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion
    }
}
