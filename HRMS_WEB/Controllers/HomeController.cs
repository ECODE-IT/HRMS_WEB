using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.DbContext;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewdataRepository viewdataRepository;
        private readonly HRMSDbContext db;

        public HomeController(ILogger<HomeController> logger, IViewdataRepository viewdataRepository, HRMSDbContext db)
        {
            _logger = logger;
            this.viewdataRepository = viewdataRepository;
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> MonthDraughtmenSummary()
        {
            var sysconfig = db.SystemSettings.FirstOrDefault();
            ViewBag.dailyTarget = sysconfig.DailyTargetHours;
            ViewBag.monthlyTarget = sysconfig.MonthlyTargetHours;
            return View(await viewdataRepository.getMonthDraughtmenReport());
        }
    }
}
