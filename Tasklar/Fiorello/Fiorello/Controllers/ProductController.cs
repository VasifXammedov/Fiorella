using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppdbContext _db;
        public ProductController(AppdbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.ProCount = _db.Products.Count();
            return View();
        }


        public IActionResult Load(int skip)
        {
            List<Product> model = _db.Products.Skip(skip).Take(8).ToList();
            return PartialView("_ProductPartial",model);

            //List<Product> model = _db.Products.Skip(skip).Take(8).ToList();
            //return PartialView("_ProductPartial", model);

            // OLD VERSION
            //return Json(_db.Products.ToList());
        }
    }
}
