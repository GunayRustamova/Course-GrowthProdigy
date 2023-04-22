using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;

namespace FinalProject.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public EmployeesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Employee's Index

        public async Task<IActionResult> Index(int page = 1)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Employees.Count() / 3);
            List<Employee> employees = await _db.Employees.
                                                 Skip((page - 1) * 3).Take(3).
                                                 Where(x => !x.IsDeactive).
                                                 Include(x => x.Course).
                                                 Include(x => x.Category).
                                                 Include(x => x.EmployeeDetail).
                                                 Include(x => x.Position).
                                                 ToListAsync();
            return View(employees);
        }

        #endregion

        #region Employee's Create

        public async Task<IActionResult> Create()
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;

            ViewBag.Positions = await _db.Positions.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Employee employee, int positionId, int courseId, int categoryId)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;

            ViewBag.Positions = await _db.Positions.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            if (courseId == null)
            {
                ModelState.AddModelError(" ", "Select a course");
                return View();
            }

            #region Create Photo
            if (employee.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please, choose a photo");
                return View();
            }
            if (!employee.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please, choose an image file");
                return View();
            }
            if (employee.Photo.IsOlderTwoMb())
            {
                ModelState.AddModelError("Photo", "Image max 2mb");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "images");
            employee.Image = await employee.Photo.SaveFileAsync(folder);
            #endregion

            employee.EmployeeDetail.DateOfBirth = DateTime.Parse(employee.EmployeeDetail.DateOfBirth.ToString(culture));
            employee.CourseId = courseId;
            employee.CategoryId = categoryId;
            employee.PositionId = positionId;
            await _db.Employees.AddAsync(employee);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Employee's Update

        public async Task<IActionResult> Update(int? id)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;

            if (id == null)
            {
                return NotFound();
            }
            Employee? dbEmployee = await _db.Employees.
                                            Include(x => x.Position).
                                            Include(x => x.EmployeeDetail).
                                            Include(x => x.Course).
                                            Include(x => x.Category).
                                            FirstOrDefaultAsync(x => x.Id == id); 
            if (dbEmployee == null)
            {
                return BadRequest();
            }
            ViewBag.Positions = await _db.Positions.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;
            return View(dbEmployee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, Employee employee, int positionId, int courseId, int categoryId)
        {

            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;

            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Positions = await _db.Positions.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).Include(x => x.Categories).ToListAsync();

            Course firstMainCategory = await _db.Courses.Include(x => x.Categories).ThenInclude(x => x.Groups).FirstOrDefaultAsync();
            ViewBag.Categories = firstMainCategory.Categories;

            Employee dbEmployee = await _db.Employees.
                                            Include(x => x.Position).
                                            Include(x => x.EmployeeDetail).
                                            Include(x => x.Course).
                                            Include(x => x.Category).
                                            FirstOrDefaultAsync(x => x.Id == id);
            if (dbEmployee == null)
            {
                return BadRequest();
            }

            #region Update Photo

            if (employee.Photo != null)
            {
                if (!employee.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Zəhmət olmasa bir şəkil fayli seçin");
                    return View(dbEmployee);
                }
                if (employee.Photo.IsOlderTwoMb())
                {
                    ModelState.AddModelError("Photo", "Şəkil max. 2 mb ola bilər");
                    return View(dbEmployee);
                }

                string folder = Path.Combine(_env.WebRootPath, "images");
                employee.Image = await employee.Photo.SaveFileAsync(folder);
                dbEmployee.Image = employee.Image;
            }
            #endregion

            dbEmployee.Name = employee.Name;
            dbEmployee.Surname = employee.Surname;
            dbEmployee.FatherName = employee.FatherName;
            dbEmployee.PositionId = positionId;
            dbEmployee.CourseId = courseId;
            dbEmployee.CategoryId = categoryId;
            dbEmployee.EmployeeDetail.Email = employee.EmployeeDetail.Email;
            dbEmployee.EmployeeDetail.Phone = employee.EmployeeDetail.Phone;
            dbEmployee.EmployeeDetail.Salary = employee.EmployeeDetail.Salary;
            dbEmployee.EmployeeDetail.Gender = employee.EmployeeDetail.Gender;
            dbEmployee.EmployeeDetail.DateOfBirth = employee.EmployeeDetail.DateOfBirth;
            dbEmployee.EmployeeDetail.Address = employee.EmployeeDetail.Address;
            dbEmployee.EmployeeDetail.StartTime = employee.EmployeeDetail.StartTime;
            dbEmployee.EmployeeDetail.ExperienceYear = employee.EmployeeDetail.ExperienceYear;
            dbEmployee.IsDeactive = employee.IsDeactive;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Employee's Detail

        public async Task<IActionResult> Detail(int? id)
        {
            CultureInfo culture = new CultureInfo("az-Latn-AZ");
            CultureInfo.CurrentCulture = culture;
            if (id == null)
            {
                return NotFound();
            }
            Employee dbEmployee = await _db.Employees.
                                            Include(x => x.EmployeeDetail).
                                            Include(x => x.Position).
                                            Include(x => x.Course).
                                            Include(x => x.Category).
                                            FirstOrDefaultAsync(x => x.Id == id);
            if (dbEmployee == null)
            {
                return BadRequest();
            }
            ViewBag.Positions = await _db.Positions.ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Categories = await _db.Categories.Where(x => !x.IsDeactive).ToListAsync();

            return View(dbEmployee);
        }

        #endregion

        #region Employee's Activity

        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee dbEmployee = await _db.Employees.
                                            Include(x => x.EmployeeDetail).
                                            Include(x => x.Course).
                                            Include(x => x.Category).
                                            Include(x => x.Position).
                                            FirstOrDefaultAsync(x => x.Id == id);
            if (dbEmployee == null)
            {
                return BadRequest();
            }
            if (dbEmployee.IsDeactive)
            {
                dbEmployee.IsDeactive = false;
            }
            else
            {
                dbEmployee.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        public async Task<IActionResult> SendMailMessage(int? id)
        {
            Employee employee = await _db.Employees.Include(x => x.EmployeeDetail).FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Categories = await _db.Categories.Where(x => !x.IsDeactive).ToListAsync();
            if (employee == null)
            {
                return BadRequest();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailMessage(MailMessage mailMessage,int? id)
        {
            Employee employee = await _db.Employees.Include(x => x.EmployeeDetail).FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            ViewBag.Courses = await _db.Courses.Where(x => !x.IsDeactive).ToListAsync();
            ViewBag.Categories = await _db.Categories.Where(x => !x.IsDeactive).ToListAsync();
            if (employee == null)
            {
                return BadRequest();
            }
            mailMessage.MailTo = employee.EmployeeDetail.Email;
            try
            {
                await Helper.SendMailAsync(mailMessage.MessageSubject, mailMessage.MessageBody, mailMessage.MailTo);
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LoadCategories(int courseId)
        {
            List<Category> categories = await _db.Categories.Where(x => x.CourseId == courseId).ToListAsync();
            return PartialView("_OnlyCategoriesPartial", categories);
        }
    }
}
