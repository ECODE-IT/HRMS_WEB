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

            
            if (model.selectedUser != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_screenshots");
                foreach(String filename in Directory.EnumerateFiles(folderpath, "*", SearchOption.TopDirectoryOnly))
                {
                    var file = filename.Substring(filename.LastIndexOf("\\") + 1);
                    String timestampstring = file.Substring(file.LastIndexOf("_") + 1, (file.Length - 1) - (file.LastIndexOf("_") + 1) - 3);
                    double timestamp = Double.Parse(timestampstring);
                    // First make a System.DateTime equivalent to the UNIX Epoch.
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);

                    // Add the number of seconds in UNIX timestamp to be converted.
                    dateTime = dateTime.AddSeconds(timestamp);

                    if (filename.Contains(model.selectedUser.Substring(0, model.selectedUser.LastIndexOf("@") - 1)) && model.selectedDate.Date == dateTime.Date)
                    {
                        model.imagePathList.Add("windows_screenshots/" + file);
                    }
                }
            }

            return View(model);
        }
    }
}
