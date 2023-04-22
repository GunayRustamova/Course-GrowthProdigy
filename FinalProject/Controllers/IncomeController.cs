using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class IncomeController : Controller
    {
      
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public IncomeController(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
       

        #region Income's Index

        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Incomes.Count() / 3);
            List<Income> incomes = _db.Incomes.Skip((page - 1) * 3).Take(3).Include(x => x.AppUser).ToList();
            return View(incomes);
        }

        #endregion

        #region Income's Create

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Income income)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cash cash = await _db.Cashes.FirstOrDefaultAsync();

            cash.LastModifiedBy = user.Name;
            cash.Balance += income.Money;
            cash.LastMotifiedMoney = income.Money;
            cash.LastModified = income.Description;
            cash.LastModifiedTime =income.IncomeTime= DateTime.UtcNow.AddHours(4);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            income.AppUserId = appUser.Id;
            await _db.Incomes.AddAsync(income);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}
