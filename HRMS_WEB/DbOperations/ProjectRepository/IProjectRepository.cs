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
        IEnumerable<Project> getUnfinishedProjects(String username);
        Task finishTheProjectById(int id);
        Task createProject(ProjectDTO projectdto);
        IEnumerable<Project> getFinishedProjectsByUsername(String userid);
    }
}
