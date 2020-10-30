using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
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

        public WindowsServiceRepository(HRMSDbContext db)
        {
            this.db = db;
        }

        public async Task<int> createDutyOnOff(string username, string password, bool isDutyOn, String sdatetime)
        {

            DateTime datetime = Convert.ToDateTime(sdatetime);

            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));

            if (user != null)
            {
                DutyLog dutyLog = new DutyLog { UserID = user.ID, IsDutyOn = isDutyOn, LogDateTime = datetime };

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
