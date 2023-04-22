using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class HistoryOfEmployeesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public HistoryOfEmployeesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Employee's Index

        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await _db.Employees.
                                                 Include(x => x.Course).
                                                 Include(x => x.Category).
                                                 Include(x => x.EmployeeDetail).
                                                 Include(x => x.Position).
                                                 ToListAsync();
            return View(employees);
        }

        #endregion

    }
}
