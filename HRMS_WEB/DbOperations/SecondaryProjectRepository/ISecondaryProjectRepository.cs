using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.SecondaryProjectRepository
{
    public interface ISecondaryProjectRepository
    {
        Task createSecondaryProject(SecondaryProject secondaryProject);
        IEnumerable<SecondaryProject> getAllProjects();
        Task createsecondaryprojectlog(SecondaryProjectLog log);
    }
}
