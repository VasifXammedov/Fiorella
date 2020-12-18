using Fiorello.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.DAL
{
    public class AppdbContext:IdentityDbContext<AppUser>
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
        {
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderContext> SliderContexts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Bio> Bios{ get; set; }
    }
}
