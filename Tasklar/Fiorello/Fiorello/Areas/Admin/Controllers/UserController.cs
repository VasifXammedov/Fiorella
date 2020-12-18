using Fiorello.DAL;
using Fiorello.Extention;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppdbContext _db;
        public UserController(UserManager<AppUser> userManager,
                               AppdbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<AppUser> users = _userManager.Users.ToList();
            List<UserVM> usersVM = new List<UserVM>();
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Username = user.UserName,
                    IsDeleted = user.IsDeleted,
                    Role = (await _userManager.GetRolesAsync(user))[0]
                };
                usersVM.Add(userVM);
            }
            //await _userManager.GetRolesAsync(users[0]);
            //return Json(usersVM);
            return View(usersVM);
        }
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeStatus")]
        public async Task<IActionResult> ChangeStatusPost(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.IsDeleted)
                user.IsDeleted = false;
            else
                user.IsDeleted = true;

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, ResetPasswordVM reset)
        {
            if (!ModelState.IsValid) return View();
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            //{
            //    ModelState.AddModelError("", "Bu merati duz yaz");
            //    return View();    
            //}

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, reset.Password);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.UserName == User.Identity.Name)
            {
                return Content("Nagarirsan eeeeeeee");
            }
            UserVM userVM =await GetUserVM(user);
            return View(userVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id,string role)
        {

            if (id == null || role==null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.UserName == User.Identity.Name)
            {
                return Content("Nagarirsan eeeeeeee");
            }
            string oldRole = (await _userManager.GetRolesAsync(user))[0];

            IdentityResult addResult= await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Bezi problemler var");
                UserVM userVM = await GetUserVM(user);
                return View(userVM);
            }

            IdentityResult removeResult= await _userManager.RemoveFromRoleAsync(user, oldRole);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Bezi problemler var");
                UserVM userVM = await GetUserVM(user);
                return View(userVM);
            }
           
            return RedirectToAction(nameof(Index));
        }
        private async Task<UserVM> GetUserVM(AppUser user)
        {
            List<string> roles = new List<string>();
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                roles.Add(item.ToString());
            }
            UserVM userVM = new UserVM
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                Username = user.UserName,
                IsDeleted = user.IsDeleted,
                Role = (await _userManager.GetRolesAsync(user))[0],
                Roles = roles
            };
            return userVM;
        }
    }
}

