using System;
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
                var result = windowsServiceRepository.validateUserByUsernamePassword(username, password).Result;

                if (result != -1)
                {
                    if(result == 0)
                    {
                        return Json(new { success = true, message = "user found successfully", islogedin = false });
                    }
                    return Json(new { success = true, message = "user found successfully", islogedin = true });
                }
                return Json(new { success = false, message = "no user found" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.StackTrace });
            }
        }

        public IActionResult CreateDutyOnOff(String username, bool isDutyOn, String punchdatetime, double powereOffTime)
        {
            try
            {
                double reslutcode = windowsServiceRepository.createDutyOnOff(username, isDutyOn, punchdatetime, powereOffTime).Result;

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
    }
}
