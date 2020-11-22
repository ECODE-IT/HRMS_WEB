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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IProjectRepository projectRepository;
        private readonly ISubLevelRepository subLevelRepository;
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public SupervisorApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
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
            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var signinresult = await signInManager.CheckPasswordSignInAsync(user, password, false);

                    if (signinresult.Succeeded)
                    {
                        return Json(new { success = true, message = "login successfull", Id = user.Id, Username = user.Name });
                    }
                    return Json(new { success = false, message = "signin failed", Id = "", Username ="" });
                }

                return Json(new { success = false, message = "signin failed", Id = "", Username = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "signin failed because " + ex.Message, Id = "", Username = "" });
            }
        }

        // supervisor get all projects api
        // GET
        public IActionResult GetAllUnfinishedProjects(String userId)
        {
            try
            {
                return Json(new { success = true, projects = projectRepository.getUnfinishedProjects(userId) });
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
        public async Task<IActionResult> SubmitSubLevel(SubLevelSubmissionDTO subLevel)
        {
            try
            {
                if (subLevel != null)
                {
                    await subLevelRepository.submitSublevelByMobile(subLevel);
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
        public async Task<IActionResult> GetDraughtmans()
        {
            try
            {
                return Ok(await userRepository.getDraughtmans());
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

        [HttpGet]
        public IActionResult GetUpComingProjects(String userid)
        {
            try
            {
                return Json(new { success = true, upcomingProjects = projectRepository.GetUpcomingProjects(userid), message = "transaction successfull" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "error occured because of " + ex.Message });
            }
        }

        public IActionResult GetSpecialTasks(String userid)
        {
            try
            {
                return Json(new { success = true, SpecialTasks = projectRepository.GetSpecialTasks(userid) });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "error because " + ex.Message});
            }
        }
    }
}
