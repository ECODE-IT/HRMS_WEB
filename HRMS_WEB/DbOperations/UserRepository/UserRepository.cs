using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.EmailRepository;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Http;
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
        private readonly IEmailSender emailsender;

        public UserRepository(HRMSDbContext db, UserManager<ApplicationUser> userManager, IEmailSender emailsender)
        {
            this.db = db;
            this.userManager = userManager;
            this.emailsender = emailsender;
        }

        public async Task approveLeave(int id, ApplicationUser user)
        {
            var leave = await db.Leaves.Include(l => l.User).Include(l => l.Approved).FirstOrDefaultAsync(l => l.ID == id);
            if(leave != null)
            {
                
                leave.IsApproved = 1;
                leave.ApprovedId = user.Id;
                db.Leaves.Update(leave);
                await db.SaveChangesAsync();

                emailsender.SendEmail(new Message(new String[] { "nikini@haritha.lk" }, "Leave respond created by ADMIN PANEL AI", $"The leave request for the user : {leave.User.Email} |{leave.User.Name}\nIS Approved by : {leave.Approved.Name} | {leave.Approved.Email}\nLeave date : {leave.Date}\n\n\n***Please don't share this email and consider this as an official and confidencial message sent on {DateTime.Now}***", null));

            } else
            {
                throw new Exception($"No leave found for the id of {id}");
            }
            
        }

        public async Task declineLeave(int id, ApplicationUser user)
        {
            var leave = await db.Leaves.Include(l => l.User).Include(l => l.Approved).FirstOrDefaultAsync(l => l.ID == id);
            if (leave != null)
            {
                leave.IsApproved = 2;
                leave.ApprovedId = user.Id;
                db.Leaves.Update(leave);
                await db.SaveChangesAsync();

                emailsender.SendEmail(new Message(new String[] { "nikini@haritha.lk" }, "Leave respond created by ADMIN PANEL AI", $"The leave request for the user : {leave.User.Email} |{leave.User.Name}\nIS Rejected by : {leave.Approved.Name} | {leave.Approved.Email}\nLeave date : {leave.Date}\n\n\n***Please don't share this email and consider this as an official and confidencial message sent on {DateTime.Now}***", null));
            }
            else
            {
                throw new Exception($"No leave found for the id of {id}");
            }
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
                .Include(p => p.SpecialTasks)
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
                            Progress = p.SubLevels.Count > 0 ? p.SubLevels.Average(sl => sl.progressFraction * 100) : 0,
                        })
                    .ToList(),
                    SpecialTasks = groupentry.Select(p => p.SpecialTasks).Aggregate((i, j) => i.Concat(j).ToList()).ToList()
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

        public async Task<bool> isdutyon(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            var dutys = db.DutyLogs.Where(dl => dl.UserId == user.Id && dl.LogDate == DateTime.Now.Date).OrderBy(dl => dl.LogDateTime).ToList();
            if (dutys == null || dutys.Count == 0)
            {
                return false;
            }

            return dutys.LastOrDefault().IsDutyOn;

        }
    }
}
