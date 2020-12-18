using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fiorello
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(20);
            });
            services.AddControllersWithViews();
            services.AddIdentity<AppUser,IdentityRole>(identityOptions=>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireNonAlphanumeric = true;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireUppercase = true;

                identityOptions.User.RequireUniqueEmail = true;

                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                identityOptions.Lockout.MaxFailedAccessAttempts = 3;
                identityOptions.Lockout.AllowedForNewUsers = true;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppdbContext>().AddErrorDescriber<AzIdentityErrorDescriber>();
            services.AddDbContext<AppdbContext>(options =>
            options.UseSqlServer(_config["ConnectionString:Default"])
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name : "areas",
                  "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                   );
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"
                    );
            });
        }
    }
}
