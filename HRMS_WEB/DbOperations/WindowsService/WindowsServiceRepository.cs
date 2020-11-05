using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.WindowsService
{
    public class WindowsServiceRepository : IWindowsServiceRepository
    {
        private readonly HRMSDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public WindowsServiceRepository(HRMSDbContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<double> createDutyOnOff(string username, bool isDutyOn, String sdatetime)
        {
            int timestamp = int.Parse(sdatetime);
            DateTime datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);

            //var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));
            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                DutyLog dutyLog = new DutyLog { UserId = user.Id, IsDutyOn = isDutyOn, LogDateTime = datetime };

                if (!isDutyOn)
                {
                    await db.DutyLogs.AddAsync(dutyLog);
                    var r = await db.SaveChangesAsync();
                    var durationleft = 0.0;
                    var logs = await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, datetime.Date) && dl.UserId.Equals(user.Id)).ToListAsync();
                    if (logs != null)
                    {
                        var dutyonhoursum = logs.Where(l => l.IsDutyOn == true).Sum(l => l.LogDateTime.TimeOfDay.TotalHours);
                        var dutyoffhoursum = logs.Where(l => l.IsDutyOn == false).Sum(l => l.LogDateTime.TimeOfDay.TotalHours);

                         durationleft = dutyoffhoursum - dutyonhoursum;
                    }
                    
                    return durationleft;
                }

                await db.DutyLogs.AddAsync(dutyLog);
                int i = await db.SaveChangesAsync();
                return 999;
            }
            return 1999;
        }

        // returns true if a user entry is exists for the username and password
        public async Task<int> validateUserByUsernamePassword(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            var lastlog = await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, DateTime.Now.Date) && dl.UserId.Equals(user.Id)).OrderByDescending(dl => dl.LogDateTime).FirstOrDefaultAsync();
            // return true if user does not exists
            if (user == null)
            {
                return -1;
            }
            if(lastlog != null && lastlog.IsDutyOn)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
