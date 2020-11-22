using HRMS_WEB.DbOperations.ProjectRepository;
using HRMS_WEB.DbOperations.SubLevelRepository;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class DraughtmanApiController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IProjectRepository projectRepository;
        private readonly ISubLevelRepository subLevelRepository;
        private readonly IUserRepository userRepository;

        public DraughtmanApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IProjectRepository projectRepository,
            ISubLevelRepository subLevelRepository,
            IUserRepository userRepository
                                       )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.projectRepository = projectRepository;
            this.subLevelRepository = subLevelRepository;
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> GetSublevelsForUserId(String userId)
        {
            try
            {
                return Json(new { success = true, sublevels = await subLevelRepository.getSublevelsForUserId(userId) });
            } 
            catch(Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> SublevelProgressUpdate(int sublevelId, bool isActive, double progressFraction)
        {
            try
            {
                await subLevelRepository.incrementSublevel(sublevelId, isActive, progressFraction);
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "error occured because of " + ex.Message });
            }
            
        }

        public IActionResult GetSublevelHistory(String userid)
        {
            try
            {
                return Json(new { success = true, sublevels = subLevelRepository.getFinishedSublevelsForuserId(userid) });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "error occured because of " + ex.Message });
            }
        }

        public IActionResult WorkedHoursSummaryReport(String userid)
        {
            return View();
        }
    }
}
