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

        public Task<IEnumerable<SubLevel>> getSubLevelListForTheUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<int> insertUser(User user)
        {
            await db.Users.AddAsync(user);
            var i = await db.SaveChangesAsync();
            return i;
        }
    }
}
