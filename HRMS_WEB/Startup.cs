using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using HRMS_WEB.Models;
using HRMS_WEB.DbContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.EmailRepository;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;

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
            services.AddSignalR();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromDays(100);//You can set Time   
            });

            services.AddControllersWithViews();

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            // MySQL DB connection service
            services.AddDbContextPool<HRMSDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // EFCore identity and set password validations  
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<HRMSDbContext>();

            // Automapper service for DTO's
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repository dependancy injection
            services.AddScoped<IWindowsServiceRepository, WindowsServiceRepository>();
            services.AddScoped<IViewdataRepository, ViewdataRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ISubLevelRepository, SubLevelRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailSender, EmailSender>();

            // https://docs.devexpress.com/XtraReports/401730/create-end-user-reporting-applications/web-reporting/asp-net-core-reporting/use-the-devexpress-cross-platform-drawing-engine
            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    DevExpress.Printing.CrossPlatform.CustomEngineHelper.RegisterCustomDrawingEngine(
            //        typeof(DevExpress.CrossPlatform.Printing.DrawingEngine.PangoCrossPlatformEngine));
            //}
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

            app.UseSession();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSignalR(route => {
                route.MapHub<ChatHub>("/Home/Index"); 
            });

            app.Use(async (context, next) =>
            {
                var hubContext = context.RequestServices
                                        .GetRequiredService<IHubContext<UserHub>>();
                //...

                if (next != null)
                {
                    await next.Invoke();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default", 
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
