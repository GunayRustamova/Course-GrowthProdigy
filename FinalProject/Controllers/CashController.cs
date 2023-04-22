using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class CashController : Controller
    {
        private readonly AppDbContext _db;
        public CashController(AppDbContext db)
        {
            _db = db;
        }

        #region Cash's Index

        public async Task<IActionResult> Index()
        {
            Cash cashes = await _db.Cashes.FirstOrDefaultAsync();
            return View(cashes);
        }

        #endregion

    }
}
