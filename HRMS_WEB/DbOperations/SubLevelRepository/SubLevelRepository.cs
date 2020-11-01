using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.SubLevelRepository
{
    public class SubLevelRepository : ISubLevelRepository
    {
        private readonly HRMSDbContext db;

        public SubLevelRepository(HRMSDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<SubLevel> getSubLevelsForProjectId(int projectId)
        {
            return db.SubLevels.AsNoTracking().Where(sl => sl.ProjectID == projectId).Include(sl => sl.User);
        }

        public async void submitSubLevel(SubLevel subLevel)
        {
            if (subLevel.ID == 0)
            {
                await db.SubLevels.AddAsync(subLevel);
            }
            else
            {
                db.SubLevels.Update(subLevel);
            }
            db.SaveChanges();
        }
    }
}
