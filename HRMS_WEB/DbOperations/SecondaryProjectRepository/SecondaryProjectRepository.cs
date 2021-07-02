using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.SecondaryProjectRepository
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SecondaryProjectRepository : ISecondaryProjectRepository
    {
        private readonly HRMSDbContext db;

        public SecondaryProjectRepository(HRMSDbContext db)
        {
            this.db = db;
        }

        public async Task createSecondaryProject(SecondaryProject secondaryProject)
        {
            await db.SecondaryProjects.AddAsync(secondaryProject);
            await db.SaveChangesAsync();
        }

        public async Task createsecondaryprojectlog(SecondaryProjectLog log)
        {
            await db.SecondaryProjectLogs.AddAsync(log);
            await db.SaveChangesAsync();
        }

        public IEnumerable<SecondaryProject> getAllProjects()
        {
            return db.SecondaryProjects.ToList();
        }

        
    }
}
