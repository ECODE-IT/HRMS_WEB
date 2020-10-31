using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HRMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SupervisorApiController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IProjectRepository projectRepository;
        private readonly ISubLevelRepository subLevelRepository;

        public SupervisorApiController(IUserRepository userRepository, IProjectRepository projectRepository, ISubLevelRepository subLevelRepository)
        {
            this.userRepository = userRepository;
            this.projectRepository = projectRepository;
            this.subLevelRepository = subLevelRepository;
        }

        public IActionResult GetAssignedSubLevelsForTheUser(String username)
        {
            if (username != null)
            {
                try
                {
                    return Ok(new { success = true, sublevels = userRepository.getSubLevelListForTheUser(username).Result });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "transaction failed because " + ex.Message });
                }
            }
            return Json(new { success = false, message = "user name not found" });
        }

        public IActionResult GetProjects()
        {
            try
            {
                return Json(new { success = true, projectList = projectRepository.getProjectList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, messaage = "error occured because " + ex.Message });
            }
        }

        public IActionResult GetSubProjectsForProjectName(int projectid)
        {
            try
            {
                return Json(new { success = true, subLevels = subLevelRepository.getSubLevelsForProjectName(projectid) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error becasue of " + ex.Message });
            }

        }

        [HttpPost]
        public IActionResult SubmitSubLevel(SubLevel subLevel)
        {
            try
            {
                if (subLevel != null)
                {
                    subLevelRepository.submitSubLevel(subLevel);
                    return Json(new { success = true, message = "creation or update is successfull" });
                }
                return Json(new { success = false, message = "object is null" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error occured becasue " + ex.Message });
            }
        }

    }
}
