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

        // returns true if a user entry is exists for the username and password
        public async Task<bool> validateUserByUsernamePassword(string username, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username) && u.UserPassword.Equals(password));
            // return true if user does not exists
            if(user == null)
            {
                return false;
            }
            return true;
        }
    }
}
