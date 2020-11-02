using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

        public async Task assignedSubLevel(SubLevelDTO subLevel)
        {
            if(subLevel.SubLevelID == 0)
            {
                throw new Exception("your sub level is not yet created");
            }
            else
            {
                var sub = await db.SubLevels.FirstOrDefaultAsync(sl => sl.ID == subLevel.SubLevelID);
                var deadline = Convert.ToDateTime(subLevel.Deadline);
                
                if(sub == null)
                {
                    throw new Exception("No sub level found for the id");
                }
                else
                {
                    sub.UserID = subLevel.UserID;
                    sub.Remark = subLevel.Remark;
                    sub.Deadline = deadline;
                    sub.ManHours = subLevel.ManHours;
                    sub.PriorityLevel = subLevel.UrgentLevel;
                    db.SubLevels.Update(sub);
                    await db.SaveChangesAsync();
                }
                
            }
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
