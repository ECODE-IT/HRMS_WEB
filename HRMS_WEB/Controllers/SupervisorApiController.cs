using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public SupervisorApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IProjectRepository projectRepository,
            ISubLevelRepository subLevelRepository,
            IUserRepository userRepository,
            IHostingEnvironment hostingEnvironment
                                       )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.projectRepository = projectRepository;
            this.subLevelRepository = subLevelRepository;
            this.userRepository = userRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // supervisor authentication api
        // GET
        public async Task<IActionResult> LoginSupervisor(String username, String password)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signinresult = await signInManager.CheckPasswordSignInAsync(user, password, false);

                if (signinresult.Succeeded)
                {
                    return Json(new { success = true, message = "login successfull" });
                }
                return Json(new { success = false, message = "signin failed" });
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
                return Json(new { success = true, sublevels = subLevelRepository.getSubLevelsForProjectId(projectId) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // supervisor submit sub level 
        // POST
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

        // finish the project by the supervisor
        // GET
        public async Task<IActionResult> FinishProject(int projectId)
        {
            try
            {
                await projectRepository.finishTheProjectById(projectId);
                return Json(new { success = true, message = "project finished successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "project is not finished becasue of " + ex.Message });
            }
        }

        // create a project by the supervisor
        // POST
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDTO projectDto)
        {
            try
            {
                await projectRepository.createProject(projectDto);
                return Json(new { success = true, message = "created project successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "failed to create the project because of " + ex.Message });
            }
        }

        // get all of the users
        // GET
        [HttpGet]
        public IActionResult GetUsersList()
        {
            try
            {
                return Ok(userRepository.getUsersWithData());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "transaction failed because of " + ex.Message });
            }
            
        }

        // assigned the sublevel assigned for the project by the supervisor
        // POST
        public async Task<IActionResult> AssignSubLevelData(SubLevelDTO subLevel)
        {
            if (subLevel != null)
            {
                try
                {
                    await subLevelRepository.assignedSubLevel(subLevel);
                    return Json(new { success = true, message = "sub level updated successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "transaction failed because of " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "sub level is empty" });
            }
        }

        // get all the finished projects for the supervisor
        // GET
        [HttpGet]
        public IActionResult GetFinishedProjects(String userid)
        {
            try
            {
                return Json(new { success = true, projects = projectRepository.getFinishedProjectsByUsername(userid) });
            }        
            catch(Exception ex)
            {
                return Json(new { success = false, message = "error occured because " + ex });
            }
        }
    }
}
