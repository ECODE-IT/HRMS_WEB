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
            if (subLevel.SubLevelID == 0)
            {
                throw new Exception("your sub level is not yet created");
            }
            else
            {
                var sub = await db.SubLevels.FirstOrDefaultAsync(sl => sl.ID == subLevel.SubLevelID);
                var deadline = Convert.ToDateTime(subLevel.Deadline);

                if (sub == null)
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

        public async Task finishSpecialTask(int id)
        {
            var specialtask = await db.specialTasks.AsNoTracking().FirstOrDefaultAsync(st => st.ID == id);
            
            if(specialtask != null)
            {
                specialtask.IsFinished = true;
                db.specialTasks.Update(specialtask);
                await db.SaveChangesAsync();
            }
        }

        public IEnumerable<SubLevel> getFinishedSublevelsForuserId(string userid)
        {
            return db.SubLevels.AsNoTracking().Where(sl => sl.Project.IsFinished == true && sl.UserID.Equals(userid));
        }

        public async Task<SubLevel> GetSubLevelForID(int sublevelId)
        {
            return await db.SubLevels.AsNoTracking().Include(sl => sl.User).FirstOrDefaultAsync(sl => sl.ID == sublevelId);
        }

        public IEnumerable<SubLevel> getSubLevelsForProjectId(int projectId)
        {
            return db.SubLevels.AsNoTracking().Where(sl => sl.ProjectID == projectId).Include(sl => sl.User);
        }

        public IEnumerable<SubLevel> getSubLevelsForProjectIdAndUserId(int projectid, string userId)
        {
            return db.SubLevels.AsNoTracking().Where(sl => sl.ProjectID == projectid && sl.UserID.Equals(userId));
        }

        public async Task<IEnumerable<SubLevel>> getSublevelsForUserId(string userId)
        {
            return await db.SubLevels.Where(sl => sl.UserID.Equals(userId)).Select(sl => new SubLevel { ID = sl.ID, Name = sl.Name, PriorityLevel = sl.PriorityLevel, progressFraction = sl.progressFraction, Deadline = sl.Deadline, IsActive = sl.IsActive, ManHours = sl.ManHours, Remark = sl.Remark, Project = new Project { Name = sl.Project.Name, User = new ApplicationUser { Name = sl.Project.User.Name } } }).ToListAsync();
        }

        public async Task incrementSublevel(int sublevelId, bool isActive, double progressFraction)
        {
            var sublevel = await db.SubLevels.AsNoTracking().FirstOrDefaultAsync(sl => sl.ID == sublevelId);

            if(sublevel != null)
            {
                sublevel.IsActive = isActive;
                sublevel.progressFraction = progressFraction;

                db.SubLevels.Update(sublevel);
                await db.SaveChangesAsync();
            }
            else
            {
                throw new Exception("no sublevel found");
            }

        }

        public async Task submitSubLevel(SubLevel subLevel)
        {
            if (subLevel.ID == 0)
            {
                await db.SubLevels.AddAsync(subLevel);
            }
            else
            {
                db.SubLevels.Update(subLevel);
            }
            await db.SaveChangesAsync();
        }

        public async Task submitSublevelByMobile(SubLevelSubmissionDTO subLevelSubmissionDTO)
        {
            if (subLevelSubmissionDTO.ID != 0)
            {
                var sublevelResult = await db.SubLevels.AsNoTracking().FirstOrDefaultAsync(sl => sl.ID == subLevelSubmissionDTO.ID);

                if (sublevelResult != null)
                {

                    var deadline = Convert.ToDateTime(subLevelSubmissionDTO.Deadline);

                    var updatinglevel = new SubLevel
                    {
                        ID = subLevelSubmissionDTO.ID,
                        UserID = subLevelSubmissionDTO.UserID,
                        Remark = subLevelSubmissionDTO.Remark,
                        Deadline = deadline,
                        ManHours = subLevelSubmissionDTO.ManHours,
                        PriorityLevel = subLevelSubmissionDTO.PriorityLevel,
                        progressFraction = sublevelResult.progressFraction,
                        IsActive = sublevelResult.IsActive,
                        Name = sublevelResult.Name,
                        ProjectID = sublevelResult.ProjectID
                    };
                    db.SubLevels.Update(updatinglevel);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
