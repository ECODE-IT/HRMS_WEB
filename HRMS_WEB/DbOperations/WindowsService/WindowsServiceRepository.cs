using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public WindowsServiceRepository(HRMSDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<double> createDutyOnOff(string username, bool isDutyOn, String sdatetime, int powereOffTime, int idletime, int autocadtime, int exceltime, int wordtime, bool isweb = false)
        {
            int timestamp = int.Parse(sdatetime);
            DateTime datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);

            //var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));
            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                DutyLog dutyLog = new DutyLog { UserId = user.Id, IsDutyOn = isDutyOn, LogDateTime = datetime, PowerOffMinutes = powereOffTime, idletime = idletime, autocadtime = autocadtime, exceltime = exceltime, wordtime = wordtime, LogDate = datetime.Date, IsWeb = isweb};

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

                        var poweroffsum = logs.Sum(l => l.PowerOffMinutes);

                        durationleft = dutyoffhoursum - dutyonhoursum - ((poweroffsum + 0.001) / 60);
                    }

                    return durationleft;
                }

                await db.DutyLogs.AddAsync(dutyLog);
                int i = await db.SaveChangesAsync();
                return 999;
            }
            return 1999;
        }

        public async Task createLeave(Leave leave)
        {
            if(leave.ID == 0)
            {
                await db.Leaves.AddAsync(leave);
                await db.SaveChangesAsync();
            } else
            {
                throw new Exception("Already have an id for the submitted value");
            }

           
        }

        public async Task<double> getAutocadHours(string userId, DateTime date)
        {
            var totalAutoCadTime = await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, date.Date) && dl.UserId.Equals(userId)).SumAsync(dl => dl.autocadtime);
            return (totalAutoCadTime) / 60.0;
        }

        public async Task<double> getidleHours(string userId, DateTime date)
        {
            var totalIdelTime = await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, date.Date) && dl.UserId.Equals(userId)).SumAsync(dl => dl.idletime);
            return (totalIdelTime) / 60.0;
        }

        public IEnumerable<Leave> GetLastLeaves(string userId)
        {
            return db.Leaves.Where(l => l.UserId.Equals(userId)).Include(l => l.Approved);
        }

        public async Task<double> getworkedHours(string userId, DateTime date)
        {
            var dutylogs = await db.DutyLogs.Where(dl => DateTime.Equals(date.Date, dl.LogDateTime.Date) && dl.UserId.Equals(userId)).ToListAsync();

            //var /*workedhours*/ = dutylogs.Sum(dl => (dl.LogDateTime.TimeOfDay.TotalHours - dl.PowerOffMinutes));

            var dutyonlogs = dutylogs.Where(dl => dl.IsDutyOn == true).ToArray();
            var dutyofflogs = dutylogs.Where(dl => dl.IsDutyOn == false).ToArray();

            var dutyonsum = dutyonlogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);
            var dutyoffsum = dutyofflogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);

            var poweroffsum = dutylogs.Sum(dl => dl.PowerOffMinutes);

            if (dutyonlogs.Length == dutyofflogs.Length)
            {
                return dutyoffsum - dutyonsum - ((poweroffsum) / 60.0);
            }
            else
            {
                return dutyoffsum - dutyonsum + DateTime.Now.TimeOfDay.TotalHours - ((poweroffsum) / 60.0);
            }

        }

        // returns true if a user entry is exists for the username and password
        public async Task<int> validateUserByUsernamePassword(ApplicationUser user, string username, string password)
        {

            if (user != null)
            {
                var signinresult = await signInManager.CheckPasswordSignInAsync(user, password, false);

                if (signinresult.Succeeded)
                {
                    var lastlogentries = db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, DateTime.Now.Date) && dl.UserId.Equals(user.Id));

                    DutyLog lastlog = null;

                    if(lastlogentries != null)
                    {
                        lastlog = await lastlogentries.OrderByDescending(dl => dl.LogDateTime).FirstOrDefaultAsync();
                    }

                    if (lastlog != null && lastlog.IsDutyOn)
                    {
                        return 1;
                    }

                    return 0;

                }
                else
                {
                    return -2;
                }

            }
            else
            {
                return -1;
            }

        }
    }
}
