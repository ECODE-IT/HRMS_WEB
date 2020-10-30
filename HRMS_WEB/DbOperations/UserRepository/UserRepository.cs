using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly HRMSDbContext db;

        public UserRepository(HRMSDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<SubLevel>> getSubLevelListForTheUser(string username)
        {
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(username.ToLower()));
            if (user != null)
            {
                var userid = user.ID;
                var projectList = db.SubLevels.AsNoTracking().Where(sl => sl.UserID == userid);
                return projectList;
            }
            return null;
        }

        public async Task<int> insertUser(User user)
        {
            await db.Users.AddAsync(user);
            var i = await db.SaveChangesAsync();
            return i;
        }
    }
}
