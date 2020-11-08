using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(HRMSDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public IEnumerable<UsersDTO> getBasicUserList()
        {
            return userManager.Users.Select(u => new UsersDTO { Username = u.Email});
        }

        public IEnumerable<UsersDTO> getBasicUserListContainsId()
        {
            return userManager.Users.Select(u => new UsersDTO { Username = u.Email, ID = u.Id });
        }

        public Task<IEnumerable<SubLevel>> getSubLevelListForTheUser(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UsersDTO> getUsersWithData()
        {
            return userManager.Users.Select(u => new UsersDTO { 
                ID = u.Id, 
                PhoneNumber = u.PhoneNumber, 
                Username = u.UserName, 
                ManHoursSum = db.SubLevels.Where(sl => sl.UserID.Equals(u.Id)).Sum(sl => sl.ManHours),
                LeftProjectCount = db.SubLevels.Where(sl => sl.UserID.Equals(u.Id)).Count()
            });
        }

        public async Task<int> insertUser(User user)
        {
            await db.Users.AddAsync(user);
            var i = await db.SaveChangesAsync();
            return i;
        }
    }
}
