using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.Models;
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
        private readonly IWindowsServiceRepository windowsServiceRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;

        public WindowsServiceController(IWindowsServiceRepository windowsServiceRepository, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            this.windowsServiceRepository = windowsServiceRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
        }

        public async Task<IActionResult> ValidateUserByUsernamePassword(String username, String password)
        {
            try
            {

                var user = await userManager.FindByNameAsync(username);

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
                    var workedHours = await windowsServiceRepository.getworkedHours(user.Id);
                    return Json(new { success = true, message = "user found successfully", islogedin = false, username = user.Name, workingHours = workedHours });
                }

                if (result == 1)
                {
                    var workedHours = await windowsServiceRepository.getworkedHours(user.Id);
                    return Json(new { success = true, message = "user found successfully", islogedin = true, username = user.Name, workingHours = workedHours });
                }

                return Json(new { success = false, message = "error occured", islogedin = false, username = "", workingHours = 0 });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.StackTrace, workingHours = 0, username = "" });
            }
        }

        public IActionResult CreateDutyOnOff(String username, bool isDutyOn, String punchdatetime, int powerOffTime, int idletime, int autocadtime)
        {
            try
            {
                double reslutcode = windowsServiceRepository.createDutyOnOff(username, isDutyOn, punchdatetime, powerOffTime, idletime, autocadtime).Result;

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
