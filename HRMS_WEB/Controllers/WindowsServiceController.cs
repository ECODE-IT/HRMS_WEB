﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.SecondaryProjectRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WindowsServiceController : Controller
    {
        private readonly ISecondaryProjectRepository secondaryProjectRepository;
        private readonly IWindowsServiceRepository windowsServiceRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IViewdataRepository viewdataRepository;
        private readonly HRMSDbContext db;

        public WindowsServiceController(ISecondaryProjectRepository secondaryProjectRepository,IWindowsServiceRepository windowsServiceRepository, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, IViewdataRepository viewdataRepository, HRMSDbContext db)
        {
            this.secondaryProjectRepository = secondaryProjectRepository;
            this.windowsServiceRepository = windowsServiceRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this.viewdataRepository = viewdataRepository;
            this.db = db;
        }

        public async Task<IActionResult> ValidateUserByUsernamePassword(String username, String password)
        {
            try
            {

                var sysconfig = db.SystemSettings.FirstOrDefault();
                ViewBag.dailyTarget = sysconfig.DailyTargetHours;
                ViewBag.monthlyTarget = sysconfig.MonthlyTargetHours;
                var reportviewmodel = new WorkHourReportViewModel { date = DateTime.Now, LeaveViewModels = await viewdataRepository.getMonthDraughtmenReport(DateTime.Now) };                var user = await userManager.FindByNameAsync(username);

                var result = await windowsServiceRepository.validateUserByUsernamePassword(user, username, password);

                if(result == -1)
                {
                    return Json(new { success = false, message = "no user found", workingHours = 0 });
                }

                if(result == -2)
                {
                    return Json(new { success = false, message = "incorrect password", workingHours = 0 });
                }

                if(result == 0)
                {
                    var workedHours = await windowsServiceRepository.getworkedHours(user.Id, DateTime.Now);
                    return Json(new { success = true, message = "user found successfully", islogedin = false, username = user.Name, workingHours = workedHours, userid = user.Id, userreport = reportviewmodel });
                }

                if (result == 1)
                {
                    var workedHours = await windowsServiceRepository.getworkedHours(user.Id, DateTime.Now);
                    return Json(new { success = true, message = "user found successfully", islogedin = true, username = user.Name, workingHours = workedHours, userid = user.Id, userreport = reportviewmodel });
                }

                return Json(new { success = false, message = "error occured", islogedin = false, username = "", workingHours = 0 });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.StackTrace, workingHours = 0, username = "" });
            }
        }

        public IActionResult CreateDutyOnOff(String username, bool isDutyOn, String punchdatetime, int powerOffTime = 0, int idletime = 0, int autocadtime = 0, int exceltime = 0, int wordtime = 0, bool IsWeb = false)
        {
            try
            {
                double reslutcode = windowsServiceRepository.createDutyOnOff(username, isDutyOn, punchdatetime, powerOffTime, idletime, autocadtime, exceltime, wordtime, isweb: IsWeb).Result;

                if(reslutcode == 1999)
                {
                    return Json(new { success = false, message = "user not found", workedhours = reslutcode });
                }

                return Json(new { success = true, message = "punch successfull", workedhours = reslutcode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "punch unsuccessful because " + ex.Message , workedhours = -1 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null)
            {
                
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_screenshots");
                var filepath = Path.Combine(folderpath, file.FileName);
                using (Stream fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult UploadLog(IFormFile file)
        {
            if (file != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_errorlogs");
                var filepath = Path.Combine(folderpath, file.FileName);
                file.CopyTo(new FileStream(filepath, FileMode.Create));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult UploadMouseLog(IFormFile file)
        {
            if (file != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_mouselogs");
                var filepath = Path.Combine(folderpath, file.FileName);
                file.CopyTo(new FileStream(filepath, FileMode.Create));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAppLog(IFormFile file)
        {
            if (file != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_applogs");
                var filepath = Path.Combine(folderpath, file.FileName);
                using (Stream fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeave(LeaveDTO leave)
        {
            try
            {
                int timestamp = int.Parse(leave.Date);
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);

                await windowsServiceRepository.createLeave(new Leave { IsApproved = leave.IsApproved, Reason = leave.Reason, UserId = leave.UserId, Date = dateTime });
                return Json(new { success = true, message = "successfully requested your leave"});
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "an error occured because of " + ex.Message });
            }
        }

        public IActionResult GetAllLeaveRequests(String userId)
        {
            try
            {
                return Json(new { success = true, leaves = windowsServiceRepository.GetLastLeaves(userId) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "an error occured because of " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitProjectShift(SecondaryProjectLog log)
        {
            try
            {
                log.LogDate = DateTime.Now.Date;
                log.LogDateTime = DateTime.Now;
                await secondaryProjectRepository.createsecondaryprojectlog(log);
                return Json(new { success = true, message = "added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
