﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.WindowsService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WindowsServiceController : Controller
    {
        private readonly IWindowsServiceRepository windowsServiceRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public WindowsServiceController(IWindowsServiceRepository windowsServiceRepository, IHostingEnvironment hostingEnvironment)
        {
            this.windowsServiceRepository = windowsServiceRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult ValidateUserByUsernamePassword(String username, String password)
        {
            try
            {
                if (windowsServiceRepository.validateUserByUsernamePassword(username, password).Result)
                {
                    return Json(new { success = true, message = "user found successfully" });
                }
                return Json(new { success = false, message = "no user found" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.StackTrace });
            }
        }

        public IActionResult CreateDutyOnOff(String username, String password, bool isDutyOn, String punchdatetime)
        {
            try
            {
                int reslutcode = windowsServiceRepository.createDutyOnOff(username, isDutyOn, punchdatetime).Result;

                if(reslutcode == 0)
                {
                    return Json(new { success = false, message = "user not found" });
                }

                return Json(new { success = true, message = "punch successfull" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "punch unsuccessful because " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_screenshots");
                var filepath = Path.Combine(folderpath, file.FileName);
                file.CopyTo(new FileStream(filepath, FileMode.Create));
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
        public IActionResult UploadAppLog(IFormFile file)
        {
            if (file != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_applogs");
                var filepath = Path.Combine(folderpath, file.FileName);
                file.CopyTo(new FileStream(filepath, FileMode.Create));
                return Ok();
            }
            return NotFound();
        }
    }
}
