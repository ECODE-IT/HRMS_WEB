using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.SecondaryProjectRepository;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class SecondaryProjectController : Controller
    {
        private readonly ISecondaryProjectRepository secondaryProjectRepository;
        private static IEnumerable<SecondaryProject> projectList;

        public SecondaryProjectController(ISecondaryProjectRepository secondaryProjectRepository)
        {
            this.secondaryProjectRepository = secondaryProjectRepository;
        }

        public IActionResult Index()
        {
            projectList = secondaryProjectRepository.getAllProjects();
            return View(projectList);
        }

        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(SecondaryProject model)
        {
            await secondaryProjectRepository.createSecondaryProject(model);
            return RedirectToAction("index");
        }

        [HttpGet]
        public object GetSecondaryProjects()
        {
            return secondaryProjectRepository.getAllProjects();
        }

    }
}
