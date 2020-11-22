using AutoMapper;
using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.EmailRepository;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly HRMSDbContext db;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;

        public ProjectRepository(HRMSDbContext db, IMapper mapper, IEmailSender emailSender)
        {
            this.db = db;
            this.mapper = mapper;
            this.emailSender = emailSender;
        }

        public async Task createProject(ProjectDTO projectdto)
        {
            // delete the upcoming project record first
            var upcomingProject = await db.UpcomingProjects.AsNoTracking().FirstOrDefaultAsync(up => up.ID == projectdto.ID);
            if (upcomingProject != null)
            {
                db.UpcomingProjects.Remove(upcomingProject);
                await db.SaveChangesAsync();

                // cast to a new project and add to ongoing line
                Project project = new Project {UserId = upcomingProject.AssigedUserId,Code = upcomingProject.Code, Deadline = upcomingProject.Deadline, Remarks = upcomingProject.Remark, Name = upcomingProject.Name, Customer = upcomingProject.Customer };

                await db.Projects.AddAsync(project);
                await db.SaveChangesAsync();

                var subLevelList = new List<SubLevel>();

                if (projectdto.SubLevelNameList != null && project.ID != 0)
                {
                    foreach (var subTitle in projectdto.SubLevelNameList)
                    {
                        if (!subTitle.Equals(""))
                        {
                            SubLevel subLevel = new SubLevel { Name = subTitle, progressFraction = 0, Remark = "", Deadline = DateTime.Now.AddDays(30), ManHours = 0, ProjectID = project.ID };
                            subLevelList.Add(subLevel);
                        }
                    }

                    await db.SubLevels.AddRangeAsync(subLevelList);
                    await db.SaveChangesAsync();
                }
            } else
            {
                throw new Exception("Someone already deleted the upcoming project. Some one has edited this. error!");
            }
        }

        public async Task deleteUpcomingProject(int projectId)
        {
            var project = await db.UpcomingProjects.AsNoTracking().FirstOrDefaultAsync(up => up.ID == projectId);
            if(project != null)
            {
                db.UpcomingProjects.Remove(project);
                await db.SaveChangesAsync();
            }
        }

        public async Task finishTheProjectById(int id)
        {
            var project = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);
            project.IsFinished = true;
            project.FinishedDate = DateTime.Now;
            db.Projects.Update(project);
            await db.SaveChangesAsync();
        }

        public IEnumerable<Project> getallProjects()
        {
            return db.Projects
                .AsNoTracking()
                .Where(p => p.IsFinished == false)
                .Include(p => p.User)
                .Select(p => new Project { 
                    ID = p.ID,
                    UserId = p.UserId,
                    AssignedDateTime = p.AssignedDateTime,
                    Customer = p.Customer,
                    Deadline = p.Deadline,
                    Name = p.Name,
                    Progress = p.SubLevels.Average(sl => sl.progressFraction),
                    Remarks = p.Remarks,
                    User = p.User,
                    SubLevels = p.SubLevels
                });
        }

        public IEnumerable<UpcomingProjects> getAllUpcommingProjects()
        {
            return db.UpcomingProjects.AsNoTracking().Include(p => p.AssigedUser);
        }

        public IEnumerable<ProjectDataDTO> getFinishedProjectsByUsername(string userid)
        {
            return db.Projects.AsNoTracking()
                .Where(p => p.IsFinished == true && p.UserId.Equals(userid))
                .Select(p => new ProjectDataDTO
            {
                ID = p.ID,
                Customer = p.Customer,
                Deadline = p.Deadline,
                Name = p.Name,
                Progress = p.Progress,
                SubLevelCount = p.SubLevels.Count,
                UserCount = p.SubLevels.Select(sl => sl.User.Email).ToArray().Distinct().Count(),
                FinishedDate = p.FinishedDate
            }); ;
        }

        public IEnumerable<SpecialTask> GetSpecialTasks(string userid)
        {
            return db.specialTasks.Where(sp => sp.UserID.Equals(userid)).Include(st => st.User);
        }

        public IEnumerable<SpecialTask> GetSpecialTasksForProject(int projectId)
        {
            return db.specialTasks.Where(st => st.ProjectID == projectId).Include(st => st.User);
        }

        public IEnumerable<ProjectDTO> getUnfinishedProjectListWithId()
        {
            return db.Projects.AsNoTracking().Where(p => p.IsFinished == false).Select(p => new ProjectDTO { ID = p.ID, Name = p.Name});
        }

        public IEnumerable<ProjectDataDTO> getUnfinishedProjects(String userId)
        {
            return db.Projects
                .AsNoTracking()
                .Where(p => p.IsFinished == false && p.UserId.Equals(userId))
                .Select(p => new ProjectDataDTO {
                    ID = p.ID,
                    Customer = p.Customer,
                    Deadline = p.Deadline,
                    Name = p.Name,
                    Progress = p.SubLevels.Average(sl => sl.progressFraction),
                    SubLevelCount = p.SubLevels.Count,
                    UserCount = p.SubLevels.Select(sl => sl.User.Email).ToArray().Distinct().Count()
                });
        }

        public IEnumerable<Project> getUnfinishedProjectsForUserIDPrject(string userId)
        {
            return db.Projects.Where(p => p.UserId.Equals(userId) && p.IsFinished == false);
        }

        public IEnumerable<UpcomingProjects> GetUpcomingIsNotifiedOnlyProjectsForUserId(string userid)
        {
            return db.UpcomingProjects.AsNoTracking().Include(up => up.AssigedUser).Where(up => up.AssigedUserId != null && up.AssigedUserId.Equals(userid) && up.IsNotified == true);
        }

        public async Task<UpcomingProjects> GetUpcomingProjectById(int projectId)
        {
            return await db.UpcomingProjects.AsNoTracking().FirstOrDefaultAsync(up => up.ID == projectId);
        }

        public IEnumerable<UpcomingProjects> GetUpcomingProjects(string userid)
        {
            return db.UpcomingProjects.AsNoTracking().Include(up => up.AssigedUser).Where(up =>up.AssigedUserId != null && up.AssigedUserId.Equals(userid) && up.IsNotified == true);
        }

        public async Task notifyProject(int projectId)
        {
            var project = await db.UpcomingProjects.AsNoTracking().FirstOrDefaultAsync(up => up.ID == projectId);
            if(project != null)
            {
                project.IsNotified = true;
                db.UpcomingProjects.Update(project);
                await db.SaveChangesAsync();
            }
        }

        public async Task submitSpecialTask(SpecialTask specialTask)
        {
            await db.specialTasks.AddAsync(specialTask);
            await db.SaveChangesAsync();
            var message = new Message(new String[] { specialTask.Email }, "Special task created by ADMIN PANEL AI", $"To user : {specialTask.UserID}\nSpecial task : {specialTask.Name}\nDeadline at : {specialTask.Deadline}\n\n\n***Please don't share this email and consider this as an official and confidencial message sent on {DateTime.Now}***");
            emailSender.SendEmail(message);
        }

        public async Task submitUpcomingProject(UpcomingProjects upcomingProjects)
        {
            if(upcomingProjects.ID == 0)
            {
                await db.UpcomingProjects.AddAsync(upcomingProjects);
            }
            else
            {
                db.UpcomingProjects.Update(upcomingProjects);
            }
            await db.SaveChangesAsync();
        }
    }
}
