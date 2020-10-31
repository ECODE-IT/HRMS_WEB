using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.Entities;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [Authorize]
    public class SubLevelController : Controller
    {
        private readonly IViewdataRepository viewdataRepository;

        public SubLevelController(IViewdataRepository viewdataRepository)
        {
            this.viewdataRepository = viewdataRepository;
        }

        public IActionResult Index()
        {

            int itemcount = viewdataRepository.GetSubLevelCount().Result;
            IEnumerable<SubLevel> subLevels = viewdataRepository.GetSubLevelsForPage(1, 20).Result;

            SubLevelViewmodel subLevelViewmodel = new SubLevelViewmodel();
            subLevelViewmodel.SubLevelList = subLevels;
            subLevelViewmodel.LastpageNumber = (int)Math.Ceiling((double.Parse(itemcount.ToString()) / 20)) == 0 ? 1 : (int)Math.Ceiling((double.Parse(itemcount.ToString()) / 20));


            return View(subLevelViewmodel);
        }

        [HttpPost]
        public IActionResult Index(int pageNumber)
        {
            int itemcount = viewdataRepository.GetSubLevelCount().Result;
            IEnumerable<SubLevel> subLevels = viewdataRepository.GetSubLevelsForPage(1, 20).Result;
            return View();
        }

    }
}
