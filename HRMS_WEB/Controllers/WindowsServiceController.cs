using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.WindowsService;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class WindowsServiceController : Controller
    {
        private readonly IWindowsServiceRepository windowsServiceRepository;

        public WindowsServiceController(IWindowsServiceRepository windowsServiceRepository)
        {
            this.windowsServiceRepository = windowsServiceRepository;
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
                return Json(new { success = false, message = e.StackTrace});
            }
        }
    }
}
