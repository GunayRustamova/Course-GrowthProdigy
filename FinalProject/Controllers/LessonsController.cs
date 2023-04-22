using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FinalProject.Controllers
{
    public class LessonsController : Controller
    {
        private readonly AppDbContext _db;
        public LessonsController(AppDbContext db)
        {
            _db = db;
        }

        #region Lesson Index

        public async Task<IActionResult> Index(int page = 1)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Lessons.Count() / 3);
            List<Lesson> lessons = await _db.Lessons.
                                             Skip((page - 1) * 3).Take(3).
                                             Include(x => x.Group).
                                             Include(x => x.Category).
                                             Include(x => x.Course).
                                             ToListAsync();
            return View(lessons);
        }

        #endregion


        #region Lesson Create

        public async Task<IActionResult> Create()
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Lesson lesson, int groupId, int courseId, int categoryId)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            lesson.GroupId = groupId;
            lesson.CourseId = courseId;
            lesson.CategoryId = categoryId;
            await _db.Lessons.AddAsync(lesson);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Lesson Update

        public async Task<IActionResult> Update(int? id)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            if (id == null)
            {
                return NotFound();
            }
            Lesson? dbLesson = await _db.Lessons.
                                        Include(x => x.Group).
                                        Include(x => x.Category).
                                        Include(x => x.Course).
                                        ThenInclude(x=>x.Categories).
                                        ThenInclude(x=>x.Groups).
                                        FirstOrDefaultAsync(x => x.Id == id);
            if (dbLesson == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

          

            return View(dbLesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, Lesson lesson, int groupId, int courseId, int categoryId)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            if (id == null)
            {
                return NotFound();
            }
            Lesson dbLesson = await _db.Lessons.
                                        Include(x => x.Group).
                                        Include(x => x.Category).
                                        Include(x => x.Course).
                                        FirstOrDefaultAsync(x => x.Id == id);
            if (dbLesson == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            dbLesson.Title = lesson.Title;
            dbLesson.StartTime = lesson.StartTime;
            dbLesson.EndTime = lesson.EndTime;
            dbLesson.IsDeactive = lesson.IsDeactive;
            dbLesson.GroupId = groupId;
            dbLesson.CourseId = courseId;
            dbLesson.CategoryId = categoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Lesson Activity

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Lesson dbLesson = await _db.Lessons.
                                       Include(x => x.Group).
                                       Include(x => x.Category).
                                       Include(x => x.Course).
                                       FirstOrDefaultAsync(x => x.Id == id);
            if (dbLesson == null)
            {
                return BadRequest();
            }
            if (dbLesson.IsDeactive)
            {
                dbLesson.IsDeactive = false;
            }
            else
            {
                dbLesson.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Lesson Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Lesson dbLesson = await _db.Lessons.
                                       Include(x => x.Group).
                                       Include(x => x.Category).
                                       Include(x => x.Course).
                                       FirstOrDefaultAsync(x => x.Id == id);
            if (dbLesson == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();

            return View(dbLesson);
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
            Lesson dbLesson = await _db.Lessons.
                                       Include(x => x.Group).
                                       Include(x => x.Category).
                                       Include(x => x.Course).
                                       FirstOrDefaultAsync(x => x.Id == id);
            if (dbLesson == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            _db.Lessons.Remove(dbLesson);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Lesson LoadCategories
        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Include(x => x.Groups).Where(x => x.CourseId == courseId).ToListAsync();


            return PartialView("_CategoriesPartial", categories);
        }
        #endregion

        #region Lesson LoadGroups
        public async Task<IActionResult> LoadGroups(int categoryId)
        {
            List<Group> categoryGroups = await _db.Groups.Where(x => x.CategoryId == categoryId).ToListAsync();
            return PartialView("_GroupsPartial", categoryGroups);
        }
        #endregion

    }
}
