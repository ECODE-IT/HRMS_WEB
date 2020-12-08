
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
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class SubLevelController : Controller
    {
        private readonly IViewdataRepository viewdataRepository;
        private readonly ISubLevelRepository subLevelRepository;
        private readonly IUserRepository userRepository;
        private readonly IProjectRepository projectRepository;
        private IEnumerable<UsersDTO> userlist;
        private static IEnumerable<ProjectDTO> projectList;

        public SubLevelController(IViewdataRepository viewdataRepository, ISubLevelRepository subLevelRepository, IUserRepository userRepository, IProjectRepository projectRepository)
        {
            this.viewdataRepository = viewdataRepository;
            this.subLevelRepository = subLevelRepository;
            this.userRepository = userRepository;
            this.projectRepository = projectRepository;
        }

        public IActionResult Index()
        {
            SubLevelViewmodel subLevelViewmodel = new SubLevelViewmodel();
            //userlist = userRepository.getBasicUserListContainsId();
            projectList = projectRepository.getUnfinishedProjectListWithId();
            subLevelViewmodel.ProjectList = projectList;

            return View(subLevelViewmodel);
        }

        [HttpPost]
        public IActionResult Index(SubLevelViewmodel model)
        {
            model.ProjectList = projectList;
            model.SubLevelList = subLevelRepository.getSubLevelsForProjectId(model.selectedProjectId);
            return View(model);
        }

        public IActionResult GetSublevelsForProject(int projectId)
        {
            ViewBag.projectId = projectId;
            return View(subLevelRepository.getSubLevelsForProjectId(projectId));
        }

        public async Task<IActionResult> Edit(int sublevelId)
        {
            return View(await subLevelRepository.GetSubLevelForID(sublevelId));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SubLevel model)
        {

            await subLevelRepository.submitSubLevel(model);
            return RedirectToAction("GetSublevelsForProject", new { projectId = model.ProjectID });
        }

        public async Task<IActionResult> CreateSublevelForProject(int projectId)
        {
            ViewBag.projectId = projectId;
            if (ViewBag.draughtmanlist == null)
            {
                ViewBag.draughtmanlist = await userRepository.GetDraughtmenWithOnlyIDName();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSublevelForProject(SubLevel model)
        {
            if (ModelState.IsValid)
            {
                await subLevelRepository.submitSubLevel(model);
                return RedirectToActionPermanent("Index", "Project");
            }

            return View(model);
        }

        public async Task<IActionResult> FinishSpecialTask(int id)
        {
            await subLevelRepository.finishSpecialTask(id);
            return RedirectToAction("SpecialTasks", "User");
        }

        public async Task<IActionResult> DeleteSubLevel(int id, int projectId)
        {
            await subLevelRepository.deleteSubLevel(id);
            return RedirectToAction("GetSublevelsForProject", new { projectId = projectId });
        }
    }
}
