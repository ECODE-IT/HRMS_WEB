using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.SubLevelRepository
{
    public interface ISubLevelRepository
    {
        IEnumerable<SubLevel> getSubLevelsForProjectId(int projectid);
        Task submitSubLevel(SubLevel subLevel);
        Task assignedSubLevel(SubLevelDTO subLevel);
        IEnumerable<SubLevel> getSubLevelsForProjectIdAndUserId(int projectid, String userId);
        Task<SubLevel> GetSubLevelForID(int sublevelId);
        Task submitSublevelByMobile(SubLevelSubmissionDTO subLevelSubmissionDTO);
        Task<IEnumerable<SubLevel>> getSublevelsForUserId(String userId);
        Task incrementSublevel(int sublevelId, bool isActive, double progressFraction);
        IEnumerable<SubLevel> getFinishedSublevelsForuserId(String userid);
        Task finishSpecialTask(int id);
    }
}
