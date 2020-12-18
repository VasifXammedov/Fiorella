using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly AppdbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppdbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            ViewBag.SlideCount = _context.Sliders.Count();
            return View(_context.Sliders.ToList());
        }
        public IActionResult Create()
        {
            int count = _context.Sliders.Count();
            if (count >= 5)
            {
                return Content("get redol");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            #region Failin yuklenmesi
            //if (slider.Photo == null)
            //{
            //    return View();
            //}
            //if (!slider.Photo.ContentType.Contains("image/"))
            //{
            //    ModelState.AddModelError("Photo", "Please select");
            //    return View();
            //}
            //if (slider.Photo.Length / 1024 > 200)
            //{
            //    ModelState.AddModelError("Photo", "Max image lenght most be 200kb");
            //    return View();
            //}
            //int count = _context.Sliders.Count();
            //if (count >= 5)
            //{
            //    return Content("get redol");
            //}
            ////return Content((slider.Photo.Length/1024).ToString());

            ////Burda bashqa yerden sekilin getirilmesidir.(Using) kimse mudaxile ede bilmesin

            //string failName = Guid.NewGuid().ToString() + slider.Photo.FileName;
            //string path = Path.Combine(_env.WebRootPath, "img", failName);

            //using (FileStream fileStream = new FileStream(path + failName, FileMode.Create))
            //{
            //    await slider.Photo.CopyToAsync(fileStream);
            //}
            //slider.Image = failName;
            //await _context.Sliders.AddAsync(slider);
            //await _context.SaveChangesAsync();
            #endregion

            #region MultiFile Upload
            if (slider.Photos == null)
            {
                return View();
            }
            foreach (IFormFile photo in slider.Photos)
            {
              
                if (!photo.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Photo", "Please select");
                    return View();
                }
                if (photo.Length / 1024 > 200)
                {
                    ModelState.AddModelError("Photo", "Max image lenght most be 200kb");
                    return View();
                }
               
                //return Content((slider.Photo.Length/1024).ToString());

                //Burda bashqa yerden sekilin getirilmesidir.(Using) kimse mudaxile ede bilmesin

                string failName = Guid.NewGuid().ToString() + slider.Photo.FileName;
                string path = Path.Combine(_env.WebRootPath, "img", failName);

                using (FileStream fileStream = new FileStream(path + failName, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                slider.Image = failName;
                await _context.Sliders.AddAsync(slider);
               
            }
            await _context.SaveChangesAsync();
            #endregion

            return RedirectToAction(nameof(Index));
           

        }

        // Slider-e elave etdiyimiz shekili silmek uchun...
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider slider =await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("get redol");
            }
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("get redol");
            }

            //string path = Path.Combine(_env.WebRootPath, "img", slider.Image);
            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}

            bool isDelete= Helper.DeleteImage(_env.WebRootPath, "img", slider.Image);

            if (!isDelete)
            {
                ModelState.AddModelError("", "Some problem");
                return View(slider);
            } 

             _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
