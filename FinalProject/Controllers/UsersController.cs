using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;

namespace FinalProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _env = env;
        }

        #region User's Index

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_db.Users.Count() / 3);
            List<AppUser> users = await _userManager.Users.Skip((page - 1) * 3).Take(3).ToListAsync();
            List<UserVM> userVMs = new List<UserVM>();
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.UserName,
                    IsDeactive = user.IsDeactive,
                    Image = user.Image,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                };
                userVMs.Add(userVM);
            }

            return View(userVMs);
        }

        #endregion

        #region User's Create

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVM registerVM, string role)
        {
            
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            #region Create Photo
            if (registerVM.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please, choose a photo");
                return View();
            }
            if (!registerVM.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please, choose an image file");
                return View();
            }
            if (registerVM.Photo.IsOlderTwoMb())
            {
                ModelState.AddModelError("Photo", "Image max 2mb");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "images");
            registerVM.Image = await registerVM.Photo.SaveFileAsync(folder);
            #endregion

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email,
                UserName = registerVM.Username,
                Image = registerVM.Image,
            };
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser, role);

            return RedirectToAction("Index");
        }
        #endregion

        #region User's Update

        public async Task<IActionResult> Update(string id)
        {
           

            if (User.Identity.IsAuthenticated)
            {
            }
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            UpdateVM dbUpdateVM = new UpdateVM
            {
                Gender = user.Gender,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Username = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),


            };
            
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(dbUpdateVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, UpdateVM updateVM, string newRole, bool Gender)
        {
           
            #region From Get
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            UpdateVM dbUpdateVM = new UpdateVM
            {
                Gender = user.Gender,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Username = user.UserName,
                Image= user.Image,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
            };
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            #endregion
           
            #region Update Photo

            if (updateVM.Photo != null)
            {
                if (!updateVM.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Zəhmət olmasa bir şəkil fayli seçin");
                    return View(dbUpdateVM);
                }
                if (updateVM.Photo.IsOlderTwoMb())
                {
                    ModelState.AddModelError("Photo", "Şəkil max. 2 mb ola bilər");
                    return View(dbUpdateVM);
                }

                string folder = Path.Combine(_env.WebRootPath, "images");
                updateVM.Image = await updateVM.Photo.SaveFileAsync(folder);
                dbUpdateVM.Image = updateVM.Image;
            }

            #endregion

            user.Email = updateVM.Email;
            user.UserName = updateVM.Username;
            user.Name = updateVM.Name;
            user.Surname = updateVM.Surname;
            user.Gender = Gender;
            user.Image= updateVM.Image;


            await _userManager.UpdateAsync(user);

            if (newRole != dbUpdateVM.Role)
            {
                IdentityResult addIdentityResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addIdentityResult.Succeeded)
                {
                    foreach (IdentityError error in addIdentityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                IdentityResult removeIdentityResult = await _userManager.RemoveFromRoleAsync(user, dbUpdateVM.Role);
                if (!removeIdentityResult.Succeeded)
                {
                    foreach (IdentityError error in removeIdentityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region User's Reset Password

        public async Task<IActionResult> ResetPassword(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, ResetPasswordVM resetPasswordVM, string newRole)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            #region From Get

            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }

            #endregion

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region User's Activity

        public async Task<IActionResult> Activity(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.IsDeactive)
            {
                user.IsDeactive = false;
            }
            else
            {
                user.IsDeactive = true;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");

        }

        #endregion
    }
}