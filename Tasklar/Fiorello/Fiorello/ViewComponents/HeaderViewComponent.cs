using Fiorello.DAL;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppdbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(AppdbContext context,
                                  UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.BasketCount = 0;
            ViewBag.UserFulname = String.Empty;
            if (User.Identity.IsAuthenticated)
            {
                string fullname = (await _userManager.FindByNameAsync(User.Identity.Name)).Fullname;
                ViewBag.UserFulname = fullname;
            }
            if (Request.Cookies["basket"] != null)
            {
                List<BasketVM> baskets = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
                //ViewBag.BasketCount = baskets.Count();
                ViewBag.BasketCount = baskets.Sum(p => p.Count);

            }
            Bio model = _context.Bios.FirstOrDefault();
            
            return View(await Task.FromResult(model));
        }
    }
}
