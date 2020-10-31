using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.DbOperations.WindowsService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HRMS_WEB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // MySQL DB connection service
            services.AddDbContextPool<HRMSDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // EFCore identity and set password validations  
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<HRMSDbContext>();

            // Repository dependancy injection
            services.AddScoped<IWindowsServiceRepository, WindowsServiceRepository>();
            services.AddScoped<IViewdataRepository, ViewdataRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ISubLevelRepository, SubLevelRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
