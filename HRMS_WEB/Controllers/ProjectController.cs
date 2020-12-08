using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IViewdataRepository viewdataRepository;
        private readonly ISubLevelRepository subLevelRepository;
        private readonly IUserRepository userRepository;
        private readonly IProjectRepository projectRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private static IEnumerable<Project> projectList;
        private static IEnumerable<UsersDTO> userlist;
        private static ApplicationUser currentUser;

        public ProjectController(IViewdataRepository viewdataRepository, ISubLevelRepository subLevelRepository, IUserRepository userRepository, IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
        {
            this.viewdataRepository = viewdataRepository;
            this.subLevelRepository = subLevelRepository;
            this.userRepository = userRepository;
            this.projectRepository = projectRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            
            if (await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                projectList = projectRepository.getallProjects();
            }
            else
            {
                projectList = projectRepository.getUnfinishedProjectsForUserIDPrject(currentUser.Id);
            }
            
           

            return View(projectList);
        }

        public async Task<IActionResult> OpenProjects()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return View(projectRepository.getAllUpcommingProjects());
            }
            else if (await userManager.IsInRoleAsync(currentUser, "Supervisor"))
            {
                return View(projectRepository.GetUpcomingIsNotifiedOnlyProjectsForUserId(currentUser.Id));
            }

            return View(new List<UpcomingProjects>());

        }

        public async Task<IActionResult> UpcomingProjectEdit(int projectId)
        {
            if (userlist == null)
            {
                userlist = userRepository.getBasicUserListContainsId();
            }
            ViewBag.userList = userlist;
            return View(await projectRepository.GetUpcomingProjectById(projectId));
        }

        [HttpPost]
        public async Task<IActionResult> UpcomingProjectEdit(UpcomingProjects model)
        {
            try
            {
                await projectRepository.submitUpcomingProject(model);
                return RedirectToAction("OpenProjects");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

        }

        public IActionResult ProjectCriteria()
        {
            return View();
        }

        public IActionResult CreateUpcomingProject()
        {
            if (userlist == null)
            {
                userlist = userRepository.getBasicUserListContainsId();
            }
            ViewBag.userList = userlist;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUpcomingProject(UpcomingProjects model)
        {
            await projectRepository.submitUpcomingProject(model);
            return RedirectToAction("OpenProjects");
        }

        public async Task<IActionResult> DeleteUpcomingProject(int projectId)
        {
            await projectRepository.deleteUpcomingProject(projectId);
            return RedirectToAction("OpenProjects");
        }

        [HttpGet]
        public async Task<IActionResult> NotifyDependent(int projectId)
        {
            await projectRepository.notifyProject(projectId);
            return RedirectToAction("OpenProjects");
        }

        public async Task<IActionResult> MergeToOngoingProjects(int projectId)
        {
            await projectRepository.createProject(new ProjectDTO { ID = projectId, SubLevelNameList = new List<string>() });
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProject(int projectId)
        {
            await projectRepository.deleteProject(projectId);
            return RedirectToAction("Index");
        }
    }
}
