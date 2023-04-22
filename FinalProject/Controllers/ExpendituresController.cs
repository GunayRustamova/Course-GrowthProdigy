using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{

    public class ExpendituresController : Controller
    {

        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public ExpendituresController(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        #region Expenditure's Index

        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Expenditures.Count() / 3);
            List<Expenditure> expenditures = _db.Expenditures.Skip((page - 1) * 3).Take(3).Include(x => x.AppUser).ToList();
            return View(expenditures);
        }

        #endregion

        #region Expenditure's Create

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expenditure expenditure)
        {



            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cash cash = await _db.Cashes.FirstOrDefaultAsync();
            cash.LastModifiedBy = user.Name;

            if (expenditure.Money > cash.Balance)
            {
                ModelState.AddModelError("Money", "Balansda kifayet qeder Pul yoxdur.");
                return View();
            }
            else
            {
                cash.Balance -= expenditure.Money;
            }

            cash.LastMotifiedMoney = expenditure.Money - expenditure.Money - expenditure.Money;
            cash.LastModified = expenditure.Description;
            cash.LastModifiedTime = expenditure.ExpenditureTime = DateTime.UtcNow.AddHours(4);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            expenditure.AppUserId = appUser.Id;
            await _db.Expenditures.AddAsync(expenditure);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion
    }
}
