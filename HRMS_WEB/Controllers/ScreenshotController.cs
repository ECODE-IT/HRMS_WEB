using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [Authorize]
    public class ScreenshotController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private static IEnumerable<UsersDTO> usersList;

        public ScreenshotController(IUserRepository userRepository, IHostingEnvironment hostingEnvironment)
        {
            this.userRepository = userRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            usersList = userRepository.getBasicUserList();
            var model = new GalleryViewmodel { usersList = usersList};
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(GalleryViewmodel model)
        {
            model.usersList = usersList;

            if(model.selectedUser != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_screenshots");
                foreach(String filename in Directory.EnumerateFiles(folderpath, "*", SearchOption.TopDirectoryOnly))
                {
                    var s = filename.Substring(filename.LastIndexOf("\\") + 1);
                    model.imagePathList.Add("windows_screenshots/"  + s);
                }
            }

            return View(model);
        }
    }
}
