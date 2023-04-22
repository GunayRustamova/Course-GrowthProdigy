using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinalProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _db;
        public StudentsController(AppDbContext db)
        {
            _db = db;
        }

        #region Student İndex

        public async Task<IActionResult> Index(int page = 1)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Students.Count() / 3);
            List<Student> students = await _db.Students.
                                          Skip((page - 1) * 3).Take(3).
                                          Where(x => !x.IsDeactive).
                                          Include(x => x.StudentDetail).
                                          Include(x => x.Group).
                                          Include(x => x.Course).
                                          Include(x => x.Category).
                                          ToListAsync();
            return View(students);
        }

        #endregion

        #region Student Create
        public async Task<IActionResult> Create()
        {
           
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();
            ViewBag.Groups = await _db.Groups.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

           
            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Student student)
        {
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();
            ViewBag.Groups = await _db.Groups.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;
            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            var groupName = _db.Groups.FirstOrDefault(x => x.Id == student.GroupId);
            if (groupName.EmptyPlaceCount != 0)
            {
                await _db.Students.AddAsync(student);
                groupName.EmptyPlaceCount = groupName.EmptyPlaceCount - 1;
                await _db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("GroupId", "Select a different group, because this group is full");
                return View();
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Student Update

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Student dbStudent = await _db.Students.
                                          Include(x => x.StudentDetail).
                                          Include(x => x.Category).
                                          Include(x => x.Course).
                                          Include(x => x.Group).
                                          FirstOrDefaultAsync(x => x.Id == id);
            if (dbStudent == null)
            {
                return BadRequest();
            }

            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();
            ViewBag.Groups = await _db.Groups.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            return View(dbStudent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, Student student, int groupId, int courseId, int categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Student dbStudent = await _db.Students.
                                          Include(x => x.StudentDetail).
                                          Include(x => x.Category).
                                          Include(x => x.Group).
                                          Include(x => x.Course).
                                          FirstOrDefaultAsync(x => x.Id == id);
            if (dbStudent == null)
            {
                return BadRequest();
            }


            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();
            ViewBag.Groups = await _db.Groups.Where(x => !x.IsDeactive).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;
            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;

            dbStudent.Name = student.Name;
            dbStudent.Surname = student.Surname;
            dbStudent.StudentDetail.FatherName = student.StudentDetail.FatherName;
            dbStudent.StudentDetail.Gender = student.StudentDetail.Gender;
            dbStudent.StudentDetail.DateOfBirth = student.StudentDetail.DateOfBirth;
            dbStudent.StudentDetail.Address = student.StudentDetail.Address;
            dbStudent.IsDeactive = student.IsDeactive;
            dbStudent.GroupId = groupId;
            dbStudent.CourseId = courseId;
            dbStudent.CategoryId = categoryId;


            var groupName = _db.Groups.FirstOrDefault(x => x.Id == student.GroupId);
            if (groupName != null && groupName.Id == student.GroupId && groupName.EmptyPlaceCount != 0)
            {
                groupName.EmptyPlaceCount = groupName.EmptyPlaceCount - 1;
                dbStudent.Group.EmptyPlaceCount = dbStudent.Group.EmptyPlaceCount + 1;
                await _db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("GroupId", "Select a different group, because this group is full");
                await _db.Groups.ToListAsync();

                return View(student);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Student Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Student dbStudent = await _db.Students.
                                       Include(x => x.StudentDetail).
                                       Include(x => x.Category).
                                       Include(x => x.Course).
                                       Include(x => x.Group).
                                       FirstOrDefaultAsync(x => x.Id == id);
            if (dbStudent == null)
            {
                return BadRequest();
            }
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            return View(dbStudent);
        }

        #endregion

        public async Task<IActionResult> SendMailMessage(int? id)
        {
            Student student = await _db.Students.Include(x => x.StudentDetail).FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;
            if (student == null)
            {
                return BadRequest();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailMessage(MailMessage mailMessage, int? id)
        {
            Student student = await _db.Students.Include(x => x.StudentDetail).FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            ViewBag.Groups = firstMainCategory.Categories.FirstOrDefault().Groups;
            if (student == null)
            {
                return BadRequest();
            }
            mailMessage.MailTo = student.StudentDetail.Email;
            try
            {
                await Helper.SendMailAsync(mailMessage.MessageSubject, mailMessage.MessageBody, mailMessage.MailTo);
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        #region Student Activity

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Student dbStudent = await _db.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (dbStudent == null)
            {
                return BadRequest();
            }
            if (dbStudent.IsDeactive)
            {
                dbStudent.IsDeactive = false;
            }
            else
            {
                dbStudent.IsDeactive = true;
            }
            var groupName = _db.Groups.FirstOrDefault(x => x.Id == dbStudent.GroupId);
            if (groupName.Id == dbStudent.GroupId)
            {
                if (dbStudent.IsDeactive)
                {
                    groupName.EmptyPlaceCount = groupName.EmptyPlaceCount + 1;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    groupName.EmptyPlaceCount = groupName.EmptyPlaceCount - 1;
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Student LoadCategories

        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Include(x => x.Groups).Where(x => x.CourseId == courseId).ToListAsync();


            return PartialView("_CategoriesPartial", categories);
        }

        #endregion

        #region Student LoadGroups

        public async Task<IActionResult> LoadGroups(int categoryId)
        {
            var categoryGroups = await _db.Groups.Where(x => x.CategoryId == categoryId).ToListAsync();
            return PartialView("_GroupsPartial", categoryGroups);
        }

        #endregion

    }
}
