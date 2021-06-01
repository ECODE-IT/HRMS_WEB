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
    [Authorize(Roles = "Admin, Supervisor")]
    public class FileController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private static IEnumerable<UsersDTO> usersList;

        public FileController(IUserRepository userRepository, IHostingEnvironment hostingEnvironment)
        {
            this.userRepository = userRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            usersList = userRepository.getBasicUserList();
            var model = new FileViewModel { usersList = usersList };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(FileViewModel model)
        {
            model.usersList = usersList;


            if (model.selectedUser != null)
            {
                var folderpath = Path.Combine(hostingEnvironment.WebRootPath, "windows_applogs");
                foreach (String filename in Directory.EnumerateFiles(folderpath, "*", SearchOption.TopDirectoryOnly))
                {
                    var file = filename.Substring(filename.LastIndexOf("/") + 1);
                    String timestampstring = file.Substring(file.LastIndexOf("_") + 1, (file.Length - 1) - (file.LastIndexOf("_") + 1) - 3);
                    int timestamp = int.Parse(timestampstring);
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);

                    if (filename.Contains(model.selectedUser.Substring(0, model.selectedUser.LastIndexOf("@") - 1)) && model.selectedDate.Date == dateTime.Date)
                    {
                        model.AppLogList.Add("windows_applogs/" + file);
                    }
                }
            }

            return View(model);
        }
    }
}
