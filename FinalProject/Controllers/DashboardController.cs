using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FinalProject.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public DashboardController(AppDbContext db)
        {
            _db= db;
        }

        public async Task<IActionResult> Index()
        {
            List<Expenditure> expenditures = await _db.Expenditures.Include(x => x.AppUser).ToListAsync();
            List<Income> incomes = await _db.Incomes.ToListAsync();
            List<Student> students = await _db.Students.Include(X => X.Group).Include(x =>x.Category).Include(x => x.Course).ToListAsync();

            DateTime currentDate = DateTime.Now;
            DateTime thirtyDaysAgo = currentDate.AddDays(-30);

            var studentsForLast30Days = students.Where(x => x.Group.StartTime >= thirtyDaysAgo && x.Group.StartTime <= currentDate);
            var expendituresForLast30Days = expenditures.Where(x => x.ExpenditureTime >= thirtyDaysAgo && x.ExpenditureTime <= currentDate);
            var incomesForLast30Days = incomes.Where(x => x.IncomeTime >= thirtyDaysAgo && x.IncomeTime <= currentDate);
            int studentCount = studentsForLast30Days.Count();
            float totalExpenditureAmount = expendituresForLast30Days.Sum(x => x.Money);
            float totalIncomeAmount = incomesForLast30Days.Sum(x => x.Money);

            
            Last30DaysVM last30DaysVM = new Last30DaysVM
            { 
                Students= studentsForLast30Days,
                Incomes= incomesForLast30Days,
                Expenditures= expendituresForLast30Days,
                StudentCount= studentCount,
                TotalExpenditureAmount= totalExpenditureAmount,
                TotalIncomeAmount = totalIncomeAmount
        };
            return View(last30DaysVM);
        }
    }
}
