using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly HRMSDbContext db;

        public ProjectRepository(HRMSDbContext db)
        {
            this.db = db;
        }

        public async Task finishTheProjectById(int id)
        {
            var project = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);
            project.IsFinished = true;
            db.Projects.Update(project);
            await db.SaveChangesAsync();
        }

        public IEnumerable<Project> getUnfinishedProjects(String username)
        {
            return db.Projects.Include(p => p.User).Where(p => p.Progress < 100 && p.User.UserName.Equals(username));
        }
    }
}
