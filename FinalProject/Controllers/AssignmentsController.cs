using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace FinalProject.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public AssignmentsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Assignments.Count() / 3);
            List<Assignment> assignments = await _db.Assignments.
                                           Skip((page - 1) * 3).Take(3).
                                           Include(x => x.CourseAssignments).
                                           ThenInclude(x => x.Course).
                                           Include(x => x.CategoryAssignments).
                                           ThenInclude(x => x.Category).
                                           Include(x => x.GroupAssignments).
                                           ThenInclude(x => x.Group).
                                           ToListAsync();
            return View(assignments);
        }

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment, int[] groupId, int[] courseId, int[] categoryId)
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            #region Create File
            if (assignment.AddFile == null)
            {
                ModelState.AddModelError("AddFile", "Please, choose a file");
                return View();
            }
            if (!assignment.AddFile.IsPdf())
            {
                ModelState.AddModelError("AddFile", "Please, choose an pdf file");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "files");
            assignment.File = await assignment.AddFile.SaveFileAsync(folder);
            #endregion

            if (groupId == null)
            {
                ModelState.AddModelError(" ", "Select group");
                return View();
            }
            List<GroupAssignment> groupAssignments = new List<GroupAssignment>();
            foreach (int item in groupId)
            {
                GroupAssignment updatedGroup = new GroupAssignment();
                updatedGroup.AssignmentId = assignment.Id;
                updatedGroup.GroupId = item;
                groupAssignments.Add(updatedGroup);
            }
            assignment.GroupAssignments = groupAssignments;

            if (courseId == null)
            {
                ModelState.AddModelError(" ", "Select course");
                return View();
            }
            List<CourseAssignment> courseAssignments = new List<CourseAssignment>();
            foreach (int item in courseId)
            {
                CourseAssignment updatedCourse = new CourseAssignment();
                updatedCourse.AssignmentId = assignment.Id;
                updatedCourse.CourseId = item;
                courseAssignments.Add(updatedCourse);
            }

            assignment.CourseAssignments = courseAssignments;

            if (categoryId == null)
            {
                ModelState.AddModelError(" ", "Select category");
                return View();
            }
            List<CategoryAssignment> categoryAssignments = new List<CategoryAssignment>();
            foreach (int item in categoryId)
            {
                CategoryAssignment updatedCategory = new CategoryAssignment();
                updatedCategory.AssignmentId = assignment.Id;
                updatedCategory.CategoryId = item;
                categoryAssignments.Add(updatedCategory);
            }

            assignment.CategoryAssignments = categoryAssignments;

            await _db.Assignments.AddAsync(assignment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Assignment? assignment = await _db.Assignments.
                                     Include(x => x.CourseAssignments).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryAssignments).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupAssignments).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (assignment == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            return View(assignment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Assignment assignment, int[] groupId, int[] courseId, int[] categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Assignment? dbAssignment = await _db.Assignments.
                                     Include(x => x.CourseAssignments).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryAssignments).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupAssignments).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (assignment == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).Include(x => x.Groups).ToListAsync();
            ViewBag.Groups = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;
            #region Update File

            if (assignment.AddFile != null)
            {
                if (!assignment.AddFile.IsPdf())
                {
                    ModelState.AddModelError("AddFile", "Zəhmət olmasa bir pdf fayli seçin");
                    return View(dbAssignment);
                }
                string folder = Path.Combine(_env.WebRootPath, "files");
                assignment.File = await assignment.AddFile.SaveFileAsync(folder);
                dbAssignment.File = assignment.File;
            }
            #endregion


            if (groupId == null)
            {
                ModelState.AddModelError(" ", "Choose group");
                return View(dbAssignment);
            }

            List<GroupAssignment> groupAssignments = new List<GroupAssignment>();
            foreach (int item in groupId)
            {
                GroupAssignment updatedGroup = new GroupAssignment();
                updatedGroup.AssignmentId = assignment.Id;
                updatedGroup.GroupId = item;
                groupAssignments.Add(updatedGroup);
            }

            dbAssignment.GroupAssignments = groupAssignments;

            if (courseId == null)
            {
                ModelState.AddModelError(" ", "Choose course");
                return View(dbAssignment);
            }

            List<CourseAssignment> courseAssignments = new List<CourseAssignment>();
            foreach (int item in courseId)
            {
                CourseAssignment updatedCourse = new CourseAssignment();
                updatedCourse.AssignmentId = assignment.Id;
                updatedCourse.CourseId = item;
                courseAssignments.Add(updatedCourse);
            }

            dbAssignment.CourseAssignments = courseAssignments;

            if (categoryId == null)
            {
                ModelState.AddModelError(" ", "Choose category");
                return View(dbAssignment);
            }

            List<CategoryAssignment> categoryAssignments = new List<CategoryAssignment>();
            foreach (int item in categoryId)
            {
                CategoryAssignment updatedCategory = new CategoryAssignment();
                updatedCategory.AssignmentId = assignment.Id;
                updatedCategory.CategoryId = item;
                categoryAssignments.Add(updatedCategory);
            }

            dbAssignment.CategoryAssignments = categoryAssignments;

            dbAssignment.Name = assignment.Name;
            dbAssignment.IsDeactive = assignment.IsDeactive;
            dbAssignment.StartTime = assignment.StartTime;
            dbAssignment.EndTime = assignment.EndTime;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Assignment dbAssignment = await _db.Assignments.FirstOrDefaultAsync(x => x.Id == id);
            if (dbAssignment == null)
            {
                return BadRequest();
            }
            if (dbAssignment.IsDeactive)
            {
                dbAssignment.IsDeactive = false;
            }
            else
            {
                dbAssignment.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Assignment? dbAssignment = await _db.Assignments.
                                     Include(x => x.CourseAssignments).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryAssignments).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupAssignments).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (dbAssignment == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            return View(dbAssignment);
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
            Assignment? dbAssignment = await _db.Assignments.
                                     Include(x => x.CourseAssignments).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryAssignments).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupAssignments).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (dbAssignment == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            _db.Assignments.Remove(dbAssignment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region LoadCategories
        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Where(x => x.CourseId == courseId).ToListAsync();
            return PartialView("_OnlyCategoriesPartial", categories);
        }

        #endregion

        #region Assignment LoadGroups

        public async Task<IActionResult> LoadGroups(int categoryId)
        {
            var categoryGroups = await _db.Groups.Where(x => x.CategoryId == categoryId).ToListAsync();
            return PartialView("_CheckBoxGroupsPartial", categoryGroups);
        }

        #endregion


        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Assignment assignment = await _db.Assignments.FirstOrDefaultAsync(x => x.Id == id);
            if (assignment == null)
            {
                return BadRequest();
            }
            string file = Path.Combine(_env.WebRootPath, "files", assignment.File);
            FileStream fileStream = new FileStream(file, FileMode.Open);
            return File(fileStream, "application/pdf");
        }
    }
}
