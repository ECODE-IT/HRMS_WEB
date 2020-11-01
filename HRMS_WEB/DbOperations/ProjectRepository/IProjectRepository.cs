using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ProjectRepository
{
    public interface IProjectRepository
    {
        IEnumerable<Project> getUnfinishedProjects(String username);
    }
}
