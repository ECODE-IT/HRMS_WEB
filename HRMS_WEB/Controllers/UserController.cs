using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.EmailRepository;
using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
   [Authorize]
    public class UserController : Controller
    {
        private readonly IViewdataRepository viewdataRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmailSender emailSender;
        private readonly IProjectRepository projectRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(IViewdataRepository viewdataRepository, IUserRepository userRepository, IEmailSender emailSender, IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
        {
            this.viewdataRepository = viewdataRepository;
            this.userRepository = userRepository;
            this.emailSender = emailSender;
            this.projectRepository = projectRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Engineers()
        {
            return View(await userRepository.getengineerDetailsWithProjectProgress());
        }

        public async Task<IActionResult> Draughtmen()
        {
            return View(await userRepository.getDraughtmenDetailsWithProjectProgress());
        }

        public IActionResult CreateSpecialtask()
        {
            if(ViewBag.usersList == null && ViewBag.projectList == null)
            {
                ViewBag.projectList = projectRepository.getUnfinishedProjectListWithId();
                ViewBag.usersList = userRepository.getBasicUserListContainsId();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialtask(SpecialTask specialTask)
        {
            if (ModelState.IsValid)
            {
                await projectRepository.submitSpecialTask(specialTask);
                return RedirectToAction("Index", "Home");
            }
            return View(specialTask);
        }

        public async Task<IActionResult> SpecialTasks()
        {
            var currentuser = await userManager.GetUserAsync(HttpContext.User);
            return View(projectRepository.GetSpecialTasks(currentuser.Id));
        }

    }
}
