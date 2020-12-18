using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.DAL;
using Fiorello.Models;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fiorello.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppdbContext _db;

        public HomeController(AppdbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            //HttpContext.Session.SetString("name", "Elgun");
            //Response.Cookies.Append("surname", "Maksudzade",new CookieOptions {MaxAge=TimeSpan.FromMinutes(20) });
            HomeVM homeVM = new HomeVM
            {
                Sliders = _db.Sliders.ToList(),
                SliderContexts = _db.SliderContexts.FirstOrDefault(),
                Categories = _db.Categories.ToList()
            };

            return View(homeVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            Product product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            List<BasketVM> basket;
            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

            }
            else
            {
                basket = new List<BasketVM>();
            }
            BasketVM isExist = basket.FirstOrDefault(p => p.Id == id);
            if (isExist == null)
            {
                basket.Add(new BasketVM
                {
                    Id = id,
                    Count = 1
                });
            }
            else
            {
                isExist.Count += 1;
            }

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Basket()
        {
            //string session = HttpContext.Session.GetString("name");
            //string cookie= Request.Cookies["surname"];
            //return Content(session+" "+cookie);
            List<BasketVM> dbBasket = new List<BasketVM>();
            ViewBag.Total = 0;
            if (Request.Cookies["basket"] != null)
            {
                List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
                foreach (BasketVM pro in basket)
                {
                    Product dbProduct = await _db.Products.FindAsync(pro.Id);
                    pro.Title = dbProduct.Title;
                    pro.Price = dbProduct.Price * pro.Count;
                    pro.Image = dbProduct.Image;
                    dbBasket.Add(pro);
                    ViewBag.Total += pro.Price;
                }
            }

            return View(dbBasket);
        }
        public IActionResult DeleteBasket(int id)
        {
            List<BasketVM> dbBasket = new List<BasketVM>();
            dbBasket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

            foreach (BasketVM pro in dbBasket.ToList())
            {

                if (id == pro.Id)
                {
                    dbBasket.Remove(pro);
                    Response.Cookies.Delete("basket");
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(dbBasket));
                }
            }

            return RedirectToAction("Basket", dbBasket);
        }
    }


}
