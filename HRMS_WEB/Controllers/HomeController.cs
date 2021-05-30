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
using System.Text;
using HRMS_WEB.Viewmodels;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using HRMS_WEB.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewdataRepository viewdataRepository;
        private readonly HRMSDbContext db;
        private readonly IHubContext<UserHub> userhub;

        public HomeController(ILogger<HomeController> logger, IViewdataRepository viewdataRepository, HRMSDbContext db, IHubContext<UserHub> userhub)
        {
            _logger = logger;
            this.viewdataRepository = viewdataRepository;
            this.db = db;
            this.userhub = userhub;
        }

        public IActionResult Index()
        {
            var userliststring = HttpContext.Session.GetString("current_userlist");
            var username = HttpContext.User.Identity.Name;

            if (userliststring != null)
            {
                var userlist = JsonConvert.DeserializeObject<List<String>>(userliststring);
                var filteruser = userlist.FirstOrDefault(name => name.Equals(username));

                if(filteruser == null)
                {
                    userlist.Add(username);
                    var userjson = JsonConvert.SerializeObject(userlist);
                    HttpContext.Session.SetString("current_userlist", userjson);
                }

                ViewBag.count = userlist.Count();
                userhub.Clients.All.SendAsync("RecieveUsercount", userlist.Count());
            } 
            else
            {
                var userlist = new List<String>();
                userlist.Add(username);
                var userjson = JsonConvert.SerializeObject(userlist);
                HttpContext.Session.SetString("current_userlist", userjson);

                ViewBag.count = userlist.Count();
                userhub.Clients.All.SendAsync("RecieveUsercount", userlist.Count());
            }

            

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
        [AllowAnonymous]
        public async Task<IActionResult> MonthDraughtmenSummary()
        {
            var sysconfig = db.SystemSettings.FirstOrDefault();
            ViewBag.dailyTarget = sysconfig.DailyTargetHours;
            ViewBag.monthlyTarget = sysconfig.MonthlyTargetHours;
            return View(new WorkHourReportViewModel { date = DateTime.Now, LeaveViewModels = await viewdataRepository.getMonthDraughtmenReport(DateTime.Now) });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> MonthDraughtmenSummary(DateTime date)
        {
            var sysconfig = db.SystemSettings.FirstOrDefault();
            ViewBag.dailyTarget = sysconfig.DailyTargetHours;
            ViewBag.monthlyTarget = sysconfig.MonthlyTargetHours;
            return View(new WorkHourReportViewModel { date = date, LeaveViewModels = await viewdataRepository.getMonthDraughtmenReport(date) });
        }

        public async Task<IActionResult> MonthDraughmentSummaryExcelExport(DateTime date)
        {
            var leaveModelList = await viewdataRepository.getMonthDraughtmenReport(date);

            StringBuilder stringBuilder = new StringBuilder();
            var sysconfig = db.SystemSettings.FirstOrDefault();
            stringBuilder.Append("Username, DailyIdleHours, DailyWorkedHours, Monthly idle Hours, Monthly Worked Hours\n");
            foreach (var leavemodel in leaveModelList)
            {

                stringBuilder.Append($"{leavemodel.Username}, {String.Format("{0:0.00}", leavemodel.DailyIdleHours)}, {String.Format("{0:0.00}", (leavemodel.TodayWorkedHoursProgress * sysconfig.DailyTargetHours / 100.0))}, {String.Format("{0:0.00}", leavemodel.MonthIdleHours)}, {String.Format("{0:0.00}", (leavemodel.MonthWorkedHoursProgress * sysconfig.MonthlyTargetHours / 100.0))}\n");
            }

            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", $"MonthlyDraughtmenSummary as at {date}.csv");

        }
        [AllowAnonymous]
        public async Task<IActionResult> UploadHumidityTempRecords(double temp, double humidity, double temp2, double humidity2)
        {
            await viewdataRepository.insertTempData(temp, humidity, temp2, humidity2);
            return Ok();
        }

        //public async Task<IActionResult> HumidityData()
        //{
        //    return View(db.HumidityDatas.OrderByDescending(hd => hd.ID).FirstOrDefault()); 
        //}

        public async Task<Humidity> GetLastHumidityEntry()
        {
            return await viewdataRepository.getlastHumidityData();
        }
    }
}
