using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbContext
{
    public class HRMSDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<User> Users { get; set; }
    }
}
