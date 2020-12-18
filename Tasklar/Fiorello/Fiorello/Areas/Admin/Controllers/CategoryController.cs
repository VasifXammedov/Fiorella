using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppdbContext _context;
        public CategoryController(AppdbContext context)
        {
            _context = context;
        }
      
        public IActionResult Index()
        {
            return View(_context.Categories.Where(c=>c.IsDeleted==false).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Model binding
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return NotFound();
            bool isExist = _context.Categories.Where(c=>c.IsDeleted==false).Any(c => c.Name.ToLower() == category.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "Burda bundan var");
                return View();
            }
            category.IsDeleted = false;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync(); //Bunu mutleq yazmaq lazimdir..
            return RedirectToAction(nameof(Index)); //return RedirectToAction("Index");-kimide yazmaq olar
        }
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        public IActionResult Update(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        public IActionResult Delete (int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c=>c.IsDeleted==false).Include(c=>c.Products).FirstOrDefault(c=>c.Id==id);
            if (category == null) return NotFound();

            // _context.Categories.Remove(category);
            //await _context.SaveChangesAsync();

            category.IsDeleted = true;
            category.DeletedTime = DateTime.Now;
            foreach (Product pro in category.Products)
            {
                pro.DeletedTime = DateTime.Now;
                pro.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
