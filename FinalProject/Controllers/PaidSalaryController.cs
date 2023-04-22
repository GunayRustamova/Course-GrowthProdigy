using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class PaidSalaryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public PaidSalaryController(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        #region Paid Salary Index

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.PaidSalaries.Count() / 3);
            List<PaidSalary> paidSalaries = await _db.PaidSalaries.Skip((page - 1) * 3).Take(3).Include(x => x.Employee).Include(x => x.AppUser).ToListAsync();
            return View(paidSalaries);
        }

        #endregion

        #region Paid Salary Create

        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _db.Employees.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(PaidSalary paidSalary, int employeeId)
        {
            ViewBag.Employees = await _db.Employees.ToListAsync();

            Employee employee = await _db.Employees.Include(x => x.EmployeeDetail).FirstOrDefaultAsync(x => x.Id == employeeId);
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cash cash = await _db.Cashes.FirstOrDefaultAsync();

            cash.LastModifiedBy = user.Name;
            cash.Balance -= (float)employee.EmployeeDetail.Salary;
            cash.LastMotifiedMoney = (float)employee.EmployeeDetail.Salary - (float)employee.EmployeeDetail.Salary - (float)employee.EmployeeDetail.Salary;

            if (paidSalary.Money > cash.Balance)
            {
                ModelState.AddModelError("Employee", "Təəssüf ki, kursun balansında kifayət qədər pul yoxdur.");
                return View();
            }
            cash.LastModifiedTime = DateTime.UtcNow.AddHours(4);
            if (employee.EmployeeDetail.Gender)
            {
                cash.LastModified = $"{employee.Name} {employee.Surname} {employee.FatherName} qızına maaş ödənilmişdir";
            }
            else
            {
                cash.LastModified = $"{employee.Name} {employee.Surname} {employee.FatherName} oğluna maaş ödənilmişdir";

            }
            paidSalary.EmployeeId = employeeId;
            paidSalary.LastModifiedTime = DateTime.UtcNow.AddHours(4);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            paidSalary.AppUserId = appUser.Id;
            paidSalary.Money = (float)employee.EmployeeDetail.Salary;
            await _db.PaidSalaries.AddAsync(paidSalary);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}
