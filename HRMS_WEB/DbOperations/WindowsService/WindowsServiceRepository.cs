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

        public async Task<int> createDutyOnOff(string username, bool isDutyOn, String sdatetime)
        {

            DateTime datetime = Convert.ToDateTime(sdatetime);

            //var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));
            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                DutyLog dutyLog = new DutyLog { UserID = user.Id, IsDutyOn = isDutyOn, LogDateTime = datetime };

                    await db.DutyLogs.AddAsync(dutyLog);
                    var i = await db.SaveChangesAsync();
                return i;
            } 
            return 0;
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
