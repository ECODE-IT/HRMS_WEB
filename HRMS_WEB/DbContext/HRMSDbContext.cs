using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbContext
{
    public class HRMSDbContext : IdentityDbContext<ApplicationUser>
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<User> Users { get; set; }      
        public DbSet<DutyLog> DutyLogs { get; set; }
        public DbSet<SubLevel> SubLevels { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UpcomingProjects> UpcomingProjects { get; set; }
        public DbSet<SpecialTask> specialTasks { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<Humidity> HumidityDatas { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Salary> Salary { get; set; }
        public DbSet<SecondaryProject> SecondaryProjects { get; set; }
        public DbSet<SecondaryProjectLog> SecondaryProjectLogs { get; set; }
    }
}
