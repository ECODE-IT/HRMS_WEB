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
                    var durationleft = 0.0;
                    var firstdutyonlog = await db.DutyLogs.FirstOrDefaultAsync(dl => DateTime.Equals(dl.LogDateTime.Date, datetime.Date) && dl.IsDutyOn == true && dl.UserId.Equals(user.Id));
                    if (firstdutyonlog != null)
                    {
                        var dutyondate = firstdutyonlog.LogDateTime;
                         durationleft = datetime.Subtract(dutyondate).TotalHours;
                    }
                    await db.DutyLogs.AddAsync(dutyLog);
                    var r = await db.SaveChangesAsync();
                    return durationleft;
                }

                await db.DutyLogs.AddAsync(dutyLog);
                int i = await db.SaveChangesAsync();
                return 999;
            }
            return 1999;
        }

        // returns true if a user entry is exists for the username and password
        public async Task<bool> validateUserByUsernamePassword(string username, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));
            // return true if user does not exists
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
