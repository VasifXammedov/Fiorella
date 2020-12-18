using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       
        public AccountController(UserManager<AppUser> userManager,
                                    SignInManager<AppUser> signInManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or wrong!!!");
                return View();
            }

            if (user.IsDeleted)
            {
                ModelState.AddModelError("", "This account blocked!!!");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = 
                await _signInManager.PasswordSignInAsync(user, login.Password, true, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Please, Try few minutes!!");
                return View(login);
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or 123 password wrong!!!");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            AppUser newUser = new AppUser
            {
                Fullname = register.Fullname,
                Email=register.Email,
                UserName=register.Username

            };
            IdentityResult identityResult= await _userManager.CreateAsync(newUser, register.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(newUser, "Member");
            await _signInManager.SignInAsync(newUser, true);
            return RedirectToAction("Index","Home");
            
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        #region Create User Role
        //public async Task CreateUserRole()
        //{
        //    if(!(await _roleManager.RoleExistsAsync("Admin")))
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    if (!(await _roleManager.RoleExistsAsync("Member")))
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });

        //}
        #endregion
    }
}
