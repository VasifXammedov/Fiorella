using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private readonly AppdbContext _context;

        public ProductViewComponent(AppdbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int take)
        {
            List<Product> products = _context.Products.Include(p=>p.Category).Take(take).ToList();
            return View(await Task.FromResult(products));
        }

    }

}
