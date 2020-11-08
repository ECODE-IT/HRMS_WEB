using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ViewdataService
{
    public class ViewdataRepository : IViewdataRepository
    {
        private readonly HRMSDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public ViewdataRepository(HRMSDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<List<DutyLog>> getDutyLogsForTheUser(string id, DateTime selectedDate)
        {
            return await db.DutyLogs.AsNoTracking().Where(dl => DateTime.Equals(dl.LogDateTime.Date, selectedDate.Date) && dl.UserId.Equals(id)).ToListAsync();
        }

        public async Task<int> GetSubLevelCount()
        {
            int count = await db.SubLevels.CountAsync();
            return count;
        }

        public async Task<IEnumerable<SubLevel>> GetSubLevelsForPage(int indexfrom, int indexto)
        {
            var sublevels = await db.SubLevels.ToListAsync();
            return sublevels.Where(sl => sl.ID >= indexfrom && sl.ID <= indexto);
        }

        public async Task<IEnumerable<User>> getUserList()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<List<IGrouping<String, DutyLog>>> GetUserRegistariesForDate(DateTime date)
        {
            var result =  await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, date.Date)).ToListAsync();
            return result.GroupBy(dl => dl.UserId).ToList(); 
        }
    }
}
