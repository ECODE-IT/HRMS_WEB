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


    }
}
