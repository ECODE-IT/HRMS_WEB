using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ProjectRepository
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectDataDTO> getUnfinishedProjects(String userId);
        Task finishTheProjectById(int id);
        Task createProject(ProjectDTO projectdto);
        IEnumerable<ProjectDataDTO> getFinishedProjectsByUsername(String userid);
        IEnumerable<ProjectDTO> getUnfinishedProjectListWithId();
        IEnumerable<UpcomingProjects> GetUpcomingProjects(String userid);
        IEnumerable<Project> getallProjects();
        IEnumerable<UpcomingProjects> getAllUpcommingProjects();
        Task<UpcomingProjects> GetUpcomingProjectById(int projectId);
        Task submitUpcomingProject(UpcomingProjects upcomingProjects);
        Task deleteUpcomingProject(int projectId);
        Task notifyProject(int projectId);
        IEnumerable<UpcomingProjects> GetUpcomingIsNotifiedOnlyProjectsForUserId(String userid);
        Task submitSpecialTask(SpecialTask specialTask);
        IEnumerable<SpecialTask> GetSpecialTasks(String userid);
        IEnumerable<SpecialTask> GetSpecialTasksForProject(int projectId);
        IEnumerable<Project> getUnfinishedProjectsForUserIDPrject(String userId);
    }
}
