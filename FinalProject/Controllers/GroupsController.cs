using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class GroupsController : Controller
    {
        private readonly AppDbContext _db;
        public GroupsController(AppDbContext db)
        {
            _db = db;
        }

        #region Group Index

        public async Task<IActionResult> Index(int page = 1)
        {

            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Groups.Count() / 3);
            List<Group> groups = await _db.Groups.
                                           Skip((page - 1) * 3).Take(3).
                                           Include(x => x.Course).
                                           Include(x => x.Category).
                                           ToListAsync();
            return View(groups);
        }

        #endregion

        #region Group Create  

        public async Task<IActionResult> Create()
        {

            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Group group, int courseId, int categoryId)
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            bool isExist = await _db.Groups.AnyAsync(x => x.GroupNumber == group.GroupNumber);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Group is already exist");
                return View();
            }
            group.CourseId = courseId;
            group.CategoryId = categoryId;
            await _db.Groups.AddAsync(group);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Group Update 

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Group dbGroup = await _db.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (dbGroup == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            return View(dbGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, Group group, int courseId, int categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Group dbGroup = await _db.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (dbGroup == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            bool isExist = await _db.Groups.AnyAsync(x => x.GroupNumber == group.GroupNumber && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Group is already exist");
                return View(dbGroup);
            }

            dbGroup.CourseId = courseId;
            dbGroup.CategoryId = categoryId;
            dbGroup.GroupNumber = group.GroupNumber;
            dbGroup.TotalPlaceCount = group.TotalPlaceCount;
            dbGroup.EmptyPlaceCount = group.EmptyPlaceCount;
            dbGroup.StartTime = group.StartTime;
            dbGroup.EndTime = group.EndTime;
            dbGroup.IsDeactive = group.IsDeactive;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Group Activity

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Group dbGroup = await _db.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (dbGroup == null)
            {
                return BadRequest();
            }
            if (dbGroup.IsDeactive)
            {
                dbGroup.IsDeactive = false;
            }
            else
            {
                dbGroup.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Group Delete 

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Group dbGroup = await _db.Groups.Include(x => x.Category).Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == id);
            if (dbGroup == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            return View(dbGroup);
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
            Group dbGroup = await _db.Groups.Include(x => x.Category).Include(x => x.Course).Include(x => x.GroupAssignments).FirstOrDefaultAsync(x => x.Id == id);

            if (dbGroup == null)
            {
                return BadRequest();
            }

            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            _db.Groups.Remove(dbGroup);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Group LoadCategories

        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Include(x => x.Groups).Where(x => x.CourseId == courseId).ToListAsync();


            return PartialView("_OnlyCategoriesPartial", categories);
        }

        #endregion

    }
}
