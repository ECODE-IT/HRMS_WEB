using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HRMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class SupervisorApiController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IProjectRepository projectRepository;
        private readonly ISubLevelRepository subLevelRepository;

        public SupervisorApiController(UserManager<IdentityUser> userManager, 
                                       SignInManager<IdentityUser> signInManager, 
                                       IProjectRepository projectRepository,
                                       ISubLevelRepository subLevelRepository
                                       )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.projectRepository = projectRepository;
            this.subLevelRepository = subLevelRepository;
        }

        // supervisor authentication api
        // GET
        public async Task<IActionResult> LoginSupervisor(String username, String password)
        {
            var user = await userManager.FindByNameAsync(username);
            var signinresult = await signInManager.CheckPasswordSignInAsync(user, password, false);

            if(signinresult.Succeeded)
            {
                return Json(new { success = true, message = "login successfull" });
            }

            return Json(new { success = false, message = "signin failed" });
        }

        // supervisor get all projects api
        // GET
        public IActionResult GetAllUnfinishedProjects(String username)
        {
            try
            {
                return Json(new { success = true, projects = projectRepository.getUnfinishedProjects(username) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // supervisor get all sublevels for the project by id
        // GET
        public IActionResult GetAllSubLevelsForProjectId(int projectId)
        {
            try
            {
                return Json(new {success = true, sublevels = subLevelRepository.getSubLevelsForProjectId(projectId) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message});
            }
        }

        // 

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
