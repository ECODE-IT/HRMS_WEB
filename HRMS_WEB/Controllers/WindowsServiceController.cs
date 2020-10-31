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
                return Json(new { success = false, message = e.StackTrace });
            }
        }

        public IActionResult CreateDutyOnOff(String username, String password, bool isDutyOn, String punchdatetime)
        {
            try
            {
                int reslutcode = windowsServiceRepository.createDutyOnOff(username, password, isDutyOn, punchdatetime).Result;

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
    }
}
