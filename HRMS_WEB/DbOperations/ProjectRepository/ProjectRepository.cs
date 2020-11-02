using AutoMapper;
using HRMS_WEB.DbContext;
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

        public ProjectRepository(HRMSDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task createProject(ProjectDTO projectdto)
        {
            Project project = mapper.Map<Project>(projectdto);

            await db.Projects.AddAsync(project);
            await db.SaveChangesAsync();

            var subLevelList = new List<SubLevel>();

            if (projectdto.SubLevelNameList != null)
            {
                foreach (var subTitle in projectdto.SubLevelNameList)
                {
                    SubLevel subLevel = new SubLevel { Name = subTitle, progressFraction = 0, UserID = projectdto.UserID, Remark = "", Deadline = DateTime.Now.AddDays(30), ManHours = 0, ProjectID = project.ID};
                    subLevelList.Add(subLevel);
                }

                await db.SubLevels.AddRangeAsync(subLevelList);
                await db.SaveChangesAsync();
            }
        }

        public async Task finishTheProjectById(int id)
        {
            var project = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);
            project.IsFinished = true;
            db.Projects.Update(project);
            await db.SaveChangesAsync();
        }

        public IEnumerable<Project> getFinishedProjectsByUsername(string userid)
        {
            return db.Projects.Where(p => p.IsFinished == true && p.UserID.Equals(userid));
        }

        public IEnumerable<Project> getUnfinishedProjects(String username)
        {
            return db.Projects.Include(p => p.User).Where(p => p.Progress < 100 && p.User.UserName.Equals(username));
        }
    }
}
