using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
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
            return userManager.Users.Select(u => new UsersDTO { Username = u.Email });
        }

        public IEnumerable<UsersDTO> getBasicUserListContainsId()
        {
            return userManager.Users.Select(u => new UsersDTO { Username = u.Name, ID = u.Id });
        }

        public async Task<IEnumerable<DraughtmanDTO>> getDraughtmans()
        {
            var draughtmen = await userManager.GetUsersInRoleAsync("Draughtman");
            return draughtmen.Select(u => new DraughtmanDTO { ID = u.Id, Name = u.Name, PhoneNumber = u.PhoneNumber, ManhourCount = db.SubLevels.Where(sl => sl.UserID.Equals(u.Id)).Sum(sl => sl.ManHours), subLevelCount = db.SubLevels.Where(sl => sl.UserID.Equals(u.Id)).Count() });

        }

        public async Task<IEnumerable<DraughtmenViewModel>> getDraughtmenDetailsWithProjectProgress()
        {
            // there is a circular reference error if only call this line try if select and project = new Project{name = sl.project.name} without include
            var subLevelGrouping = await db.SubLevels.Where(sl => sl.UserID != null).Include(sl => sl.User).Include(sl => sl.Project).ToListAsync();

            return subLevelGrouping
                .GroupBy(slg => slg.UserID)
                .Select(slentry => new DraughtmenViewModel { 
                    ID = slentry.Key,
                    Name = slentry.FirstOrDefault().User.Name,
                    TotalSublevelCount = slentry.Count(),
                    SublevelList = slentry.Select(
                        sl => new SubLevel { 
                            Name = sl.Name ?? "N/A", 
                            progressFraction = sl.progressFraction * 100,
                            Project = sl.Project
                        }).ToList()
                });
        }

        public async Task<IEnumerable<DraughtmanDTO>> GetDraughtmenWithOnlyIDName()
        {
            var users = await userManager.GetUsersInRoleAsync("Draughtman");
            return users.Select(u => new DraughtmanDTO { ID = u.Id, Name = u.Name });
        }

        public async Task<IEnumerable<EngineerDTO>> getengineerDetailsWithProjectProgress()
        {
            var projectGroupting = await db.Projects
                .Include(p => p.SubLevels)
                .Include(p => p.User)
                .ToListAsync();

            return projectGroupting
                .GroupBy(pg => pg.UserId)
                .Select(
                groupentry => new EngineerDTO
                {
                    ID = groupentry.Key,
                    Name = groupentry.FirstOrDefault().User.Name,
                    TotalProjectCount = groupentry.Count(),
                    ProjectList = groupentry
                    .Select(
                        p => new ProjectDTO {
                            Name = p.Name, 
                            Progress = p.SubLevels.Count > 0 ? p.SubLevels.Average(sl => sl.progressFraction * 100) : 0
                        })
                    .ToList()
                });
        }

        public async Task<IEnumerable<SubLevel>> getSubLevelListForTheUser(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UsersDTO> getUsersWithData()
        {
            return userManager.Users.Select(u => new UsersDTO
            {
                ID = u.Id,
                PhoneNumber = u.PhoneNumber,
                Username = u.Name,
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
