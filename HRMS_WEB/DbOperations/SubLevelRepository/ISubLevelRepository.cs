using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.SubLevelRepository
{
    public interface ISubLevelRepository
    {
        IEnumerable<SubLevel> getSubLevelsForProjectId(int projectid);
        void submitSubLevel(SubLevel subLevel);
    }
}
