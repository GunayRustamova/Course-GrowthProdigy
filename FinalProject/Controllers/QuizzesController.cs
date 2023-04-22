using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public QuizzesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Quizzes.Count() / 3);
            List<Quiz> quizzes = await _db.Quizzes.
                                 Skip((page - 1) * 3).Take(3).
                                 Include(x => x.CourseQuizzes).
                                 ThenInclude(x => x.Course).
                                 Include(x => x.CategoryQuizzes).
                                 ThenInclude(x => x.Category).
                                 Include(x => x.GroupQuizzes).
                                 ThenInclude(x => x.Group).
                                 ToListAsync();
            return View(quizzes);
        }
        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz, int[] groupId, int[] courseId, int[] categoryId)
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            #region Create File
            if (quiz.AddFile == null)
            {
                ModelState.AddModelError("AddFile", "Please, choose a file");
                return View();
            }
            if (!quiz.AddFile.IsPdf())
            {
                ModelState.AddModelError("AddFile", "Please, choose an pdf file");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "files");
            quiz.File = await quiz.AddFile.SaveFileAsync(folder);
            #endregion

            if (groupId == null)
            {
                ModelState.AddModelError(" ", "Select group");
                return View();
            }
            List<GroupQuiz> groupQuizzes = new List<GroupQuiz>();
            foreach (int item in groupId)
            {
                GroupQuiz updatedQuiz = new GroupQuiz();
                updatedQuiz.QuizId = quiz.Id;
                updatedQuiz.GroupId = item;
                groupQuizzes.Add(updatedQuiz);
            }

            quiz.GroupQuizzes = groupQuizzes;

            if (courseId == null)
            {
                ModelState.AddModelError(" ", "Select course");
                return View();
            }
            List<CourseQuiz> courseQuizzes = new List<CourseQuiz>();
            foreach (int item in courseId)
            {
                CourseQuiz updatedQuiz = new CourseQuiz();
                updatedQuiz.QuizId = quiz.Id;
                updatedQuiz.CourseId = item;
                courseQuizzes.Add(updatedQuiz);
            }

            quiz.CourseQuizzes = courseQuizzes;

            if (categoryId == null)
            {
                ModelState.AddModelError(" ", "Select category");
                return View();
            }
            List<CategoryQuiz> categoryQuizzes = new List<CategoryQuiz>();
            foreach (int item in categoryId)
            {
                CategoryQuiz updatedCategory = new CategoryQuiz();
                updatedCategory.QuizId = quiz.Id;
                updatedCategory.CategoryId = item;
                categoryQuizzes.Add(updatedCategory);
            }

            quiz.CategoryQuizzes = categoryQuizzes;

            await _db.Quizzes.AddAsync(quiz);
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
            Quiz? quiz = await _db.Quizzes.
                                   Include(x => x.CourseQuizzes).
                                   ThenInclude(x => x.Course).
                                   Include(x => x.CategoryQuizzes).
                                   ThenInclude(x => x.Category).
                                   Include(x => x.GroupQuizzes).
                                   ThenInclude(x => x.Group).
                                   FirstOrDefaultAsync(x => x.Id == id);
            if (quiz == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            return View(quiz);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Quiz quiz, int[] groupId, int[] courseId, int[] categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Quiz? dbQuiz = await _db.Quizzes.
                                     Include(x => x.CourseQuizzes).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryQuizzes).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupQuizzes).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (quiz == null)
            {
                return BadRequest();
            }
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            #region Update File

            if (quiz.AddFile != null)
            {
                if (!quiz.AddFile.IsPdf())
                {
                    ModelState.AddModelError("AddFile", "Zəhmət olmasa bir pdf fayli seçin");
                    return View(dbQuiz);
                }
                string folder = Path.Combine(_env.WebRootPath, "files");
                quiz.File = await quiz.AddFile.SaveFileAsync(folder);
                dbQuiz.File = quiz.File;
            }
            #endregion

            if (groupId == null)
            {
                ModelState.AddModelError(" ", "Select group");
                return View();
            }
            List<GroupQuiz> groupQuizzes = new List<GroupQuiz>();
            foreach (int item in groupId)
            {
                GroupQuiz updatedGroup = new GroupQuiz();
                updatedGroup.QuizId = quiz.Id;
                updatedGroup.GroupId = item;
                groupQuizzes.Add(updatedGroup);
            }
            quiz.GroupQuizzes = groupQuizzes;

            if (courseId == null)
            {
                ModelState.AddModelError(" ", "Select course");
                return View();
            }
            List<CourseQuiz> courseQuizzes = new List<CourseQuiz>();
            foreach (int item in courseId)
            {
                CourseQuiz updatedCourse = new CourseQuiz();
                updatedCourse.QuizId = quiz.Id;
                updatedCourse.CourseId = item;
                courseQuizzes.Add(updatedCourse);
            }

            quiz.CourseQuizzes = courseQuizzes;

            if (categoryId == null)
            {
                ModelState.AddModelError(" ", "Select category");
                return View();
            }
            List<CategoryQuiz> categoryQuizzes = new List<CategoryQuiz>();
            foreach (int item in categoryId)
            {
                CategoryQuiz updatedCategory = new CategoryQuiz();
                updatedCategory.QuizId = quiz.Id;
                updatedCategory.CategoryId = item;
                categoryQuizzes.Add(updatedCategory);
            }

            quiz.CategoryQuizzes = categoryQuizzes;

            dbQuiz.Name = quiz.Name;
            dbQuiz.IsDeactive = quiz.IsDeactive;
            dbQuiz.StartTime = quiz.StartTime;
            dbQuiz.EndTime = quiz.EndTime;

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
            Quiz dbQuiz = await _db.Quizzes.FirstOrDefaultAsync(x => x.Id == id);
            if (dbQuiz == null)
            {
                return BadRequest();
            }
            if (dbQuiz.IsDeactive)
            {
                dbQuiz.IsDeactive = false;
            }
            else
            {
                dbQuiz.IsDeactive = true;
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
            Quiz? dbQuiz = await _db.Quizzes.
                                   Include(x => x.CourseQuizzes).
                                   ThenInclude(x => x.Course).
                                   Include(x => x.CategoryQuizzes).
                                   ThenInclude(x => x.Category).
                                   Include(x => x.GroupQuizzes).
                                   ThenInclude(x => x.Group).
                                   FirstOrDefaultAsync(x => x.Id == id);
            if (dbQuiz == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            return View(dbQuiz);
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
            Quiz? dbQuiz = await _db.Quizzes.
                                     Include(x => x.CourseQuizzes).
                                     ThenInclude(x => x.Course).
                                     Include(x => x.CategoryQuizzes).
                                     ThenInclude(x => x.Category).
                                     Include(x => x.GroupQuizzes).
                                     ThenInclude(x => x.Group).
                                     FirstOrDefaultAsync(x => x.Id == id);
            if (dbQuiz == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            _db.Quizzes.Remove(dbQuiz);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Where(x => x.CourseId == courseId).ToListAsync();
            return PartialView("_OnlyCategoriesPartial", categories);
        }
        #region  LoadGroups

        public async Task<IActionResult> LoadGroups(int categoryId)
        {
            var categoryGroups = await _db.Groups.Where(x => x.CategoryId == categoryId).ToListAsync();
            return PartialView("_GroupsPartial", categoryGroups);
        }

        #endregion
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Quiz quiz = await _db.Quizzes.FirstOrDefaultAsync(x => x.Id == id);
            if (quiz == null)
            {
                return BadRequest();
            }
            string file = Path.Combine(_env.WebRootPath, "files", quiz.File);
            FileStream fileStream = new FileStream(file, FileMode.Open);
            return File(fileStream, "application/pdf");
        }
    }
}
